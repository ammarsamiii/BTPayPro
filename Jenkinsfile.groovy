pipeline {
    agent any

    environment {
        COMPOSE_FILE = "docker-compose.app.yml"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unit Tests') {
            steps {
                echo "▶️ Exécution des tests unitaires..."
                sh '''
                    docker run --rm \
                        -v $PWD:/src \
                        -w /src \
                        mcr.microsoft.com/dotnet/sdk:8.0 \
                        dotnet test BTPayPro.Tests/BTPayPro.Tests.csproj --logger "trx"
                '''
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

        stage('Deploy Containers') {
            steps {
                sh "docker compose -f ${COMPOSE_FILE} down || true"
                sh "docker compose -f ${COMPOSE_FILE} up -d"
            }
        }
    }

    post {
        success {
            echo "✅ Déploiement réussi !"
        }
        failure {
            echo "❌ Erreur dans le pipeline"
        }
    }
}
