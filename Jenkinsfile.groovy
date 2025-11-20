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
            echo " Déploiement réussi !"
        }
        failure {
            echo " Erreur dans le pipeline"
        }
    }
}
