pipeline {
    agent any

    environment {
        // Fichier docker-compose pour builder les images
        COMPOSE_FILE = "docker-compose.app.yml"

        // ID du credential DockerHub dans Jenkins
        DOCKERHUB_CREDENTIALS = "dockerhub"

        // Namespace Kubernetes de l'application
        K8S_NAMESPACE = "btpaypro"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unit Tests') {
            steps {
                echo "🧪 Running unit tests..."
                sh '''
                    echo "PWD: $(pwd)"
                    ls
                    ls BTPayPro.Tests || echo "BTPayPro.Tests not found"

                    dotnet test BTPayPro.Tests/BTPayPro.Tests.csproj --logger "trx"
                '''
            }
        }

        stage('SonarQube analysis') {
            steps {
                echo "🔎 Running SonarQube analysis..."
                withSonarQubeEnv('sonarqube') {
                    sh '''
                        sonar-scanner \
                          -Dsonar.projectKey=BTPayPro \
                          -Dsonar.projectName=BTPayPro \
                          -Dsonar.sources=BTPayPro,BTPayPro.Api,BTPayPro.WebUI \
                          -Dsonar.sourceEncoding=UTF-8
                    '''
                }
            }
        }

        stage('Build Docker Images') {
            steps {
                echo "🐳 Building Docker images with docker-compose..."
                sh "docker compose -f ${COMPOSE_FILE} build"
            }
        }

        stage('Push to DockerHub') {
            steps {
                echo "📤 Pushing images to DockerHub..."
                withCredentials([
                    usernamePassword(
                        credentialsId: DOCKERHUB_CREDENTIALS,
                        usernameVariable: 'DH_USER',
                        passwordVariable: 'DH_PASS'
                    )
                ]) {
                    sh '''
                        echo "$DH_PASS" | docker login -u "$DH_USER" --password-stdin

                        echo "📦 Docker images disponibles :"
                        docker images

                        docker push $DH_USER/btpaypro-api:latest
                        docker push $DH_USER/btpaypro-webui:latest

                        docker logout
                    '''
                }
            }
        }

        stage('Deploy to MicroK8s') {
            steps {
                echo "🚀 Deploying to MicroK8s cluster..."

                // kubeconfig MicroK8s stocké comme "Secret file" dans Jenkins avec l'ID "microk8s-kubeconfig"
                withCredentials([file(credentialsId: 'microk8s-kubeconfig', variable: 'KUBECONFIG_FILE')]) {
                    sh '''
                        echo "Using kubeconfig: $KUBECONFIG_FILE"
                        kubectl --kubeconfig="$KUBECONFIG_FILE" get nodes

                        # Création / mise à jour du namespace
                        kubectl --kubeconfig="$KUBECONFIG_FILE" apply -f k8s/namespace.yaml || true

                        # Déploiement de toute la stack dans le namespace
                        kubectl --kubeconfig="$KUBECONFIG_FILE" apply -f k8s/ -n ''' + '${K8S_NAMESPACE}' + '''
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Pipeline completed successfully !"
        }
        failure {
            echo "❌ Pipeline failed, check the logs."
        }
    }
}
