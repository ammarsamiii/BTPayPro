pipeline {
    agent any

    environment {
        // Fichier docker-compose pour ton application
        COMPOSE_FILE = "docker-compose.app.yml"

        // ID du credential DockerHub dans Jenkins
        DOCKERHUB_CREDENTIALS = "dockerhub"
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

                    dotnet test BTPayPro.Tests/BTPayPro.Tests.csproj --logger \"trx\"
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

                        # Les images ont déjà été construites avec ces noms
                        docker images

                        docker push $DH_USER/btpaypro-api:latest
                        docker push $DH_USER/btpaypro-webui:latest

                        docker logout
                    '''
                }
            }
        }

        stage('Deploy Containers') {
            steps {
                echo "🚀 Deploying containers with docker-compose..."
                sh "docker compose -f ${COMPOSE_FILE} down || true"
                sh "docker compose -f ${COMPOSE_FILE} up -d"
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
