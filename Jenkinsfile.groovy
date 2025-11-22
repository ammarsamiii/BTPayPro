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

        stage('SonarQube analysis') {
            steps {
                script {
                    // 'sonar-scanner' doit être le même nom que dans "Global Tool Configuration"
                    def scannerHome = tool 'sonar-scanner'

                    // 'sonarqube' doit être le même nom que dans "Configure System" > SonarQube servers
                    withSonarQubeEnv('sonarqube') {
                        sh """
                            ${scannerHome}/bin/sonar-scanner \
                              -Dsonar.projectKey=btpaypro \
                              -Dsonar.projectName=BTPayPro \
                              -Dsonar.sources=BTPayPro,BTPayPro.Api,BTPayPro.WebUI \
                              -Dsonar.sourceEncoding=UTF-8
                        """
                    }
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
            echo "  Déploiement réussi !"
        }
        failure {
            echo "  Erreur dans le pipeline"
        }
    }
}
