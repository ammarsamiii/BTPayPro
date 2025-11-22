pipeline {
    agent any

    environment {
        COMPOSE_FILE = "docker-compose.app.yml"
        DOCKERHUB_CREDENTIALS = 'dockerhub'     
        DOCKERHUB_USER = 'samii99'          
        IMAGE_API = "btpaypro-api"
        IMAGE_WEB = "btpaypro-webui"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unit Tests') {
            steps {
                echo "✔ Running unit tests..."
                sh 'dotnet test BTPayPro.Tests/BTPayPro.Tests.csproj --logger trx'
            }
        }

        stage('SonarQube analysis') {
            steps {
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
                sh "docker compose -f ${COMPOSE_FILE} build"
            }
        }

        stage('Push to DockerHub') {
            steps {
                echo "📤 Pushing images to DockerHub..."
                withCredentials([usernamePassword(credentialsId: DOCKERHUB_CREDENTIALS, usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                    sh '''
                        echo "$PASS" | docker login -u "$USER" --password-stdin

                        docker tag btpaypro-api:latest $USER/btpaypro-api:latest
                        docker tag btpaypro-webui:latest $USER/btpaypro-webui:latest

                        docker push $USER/btpaypro-api:latest
                        docker push $USER/btpaypro-webui:latest

                        docker logout
                    '''
                }
            }
        }

        stage('Deploy Containers') {
            steps {
                sh "docker compose -f ${COMPOSE_FILE} down || true"
                sh "docker compose -f ${COMPOSE_FILE} up -d"
            }
        }
    }

    post {
        success {
            echo "🚀 Pipeline completed successfully!"
        }
        failure {
            echo "❌ Pipeline failed. Check logs."
        }
    }
}
