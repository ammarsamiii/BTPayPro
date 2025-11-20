pipeline {
    agent any

    environment {
        COMPOSE_FILE = "docker-compose.app.yml"  // adapte le nom si besoin
    }

    stages {
        stage('Checkout') {
            steps {
                // Jenkins récupère le code DIRECTEMENT depuis GitHub
                checkout scm
            }
        }

        stage('Build .NET') {
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
                // Si tu as des tests :
                // sh 'dotnet test'
            }
        }

        stage('Deploy with Docker Compose') {
            steps {
                // On arrête l'ancien déploiement s'il existe
                sh "docker compose -f ${COMPOSE_FILE} down || true"
                // On (re)build les images et on relance les conteneurs
                sh "docker compose -f ${COMPOSE_FILE} up -d --build"
            }
        }
    }

    post {
        success {
            echo '✅ CI/CD OK : DB + API + WebUI déployés'
        }
        failure {
            echo '❌ Le pipeline a échoué, va voir les logs Jenkins'
        }
    }
}
