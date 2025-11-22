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
                echo "🔍 Running unit tests..."

                // Petit debug côté host (Jenkins)
                sh '''
                    echo "===== [HOST] Workspace info ====="
                    echo "PWD: $PWD"
                    echo "ls (workspace root):"
                    ls
                    echo "ls BTPayPro (if exists):"
                    ls BTPayPro || echo "⚠️ No BTPayPro directory at workspace root"
                    echo "================================="
                '''

                // Exécution des tests dans le container .NET
                sh '''
                    docker run --rm \
                        -v $PWD:/src \
                        mcr.microsoft.com/dotnet/sdk:8.0 \
                        /bin/bash -lc "
                            echo '===== [CONTAINER] ls /src =====';
                            ls /src;
                            echo '===== [CONTAINER] ls /src/BTPayPro =====';
                            ls /src/BTPayPro || echo '⚠️ /src/BTPayPro not found';
                            echo '===== [CONTAINER] dotnet test =====';
                            dotnet test /src/BTPayPro/BTPayPro.sln --logger trx
                        "
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
