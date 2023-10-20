pipeline {
    agent any
    options {
        buildDiscarder(logRotator(numToKeepStr: '5'))
    }
    environment {
        DOCKERHUB_CREDENTIALS = credentials('gitlantis-dockerhub')
    }
    stages {
        stage('Push main to dockerhub') {
            when {
               branch 'main'               
            }
            steps {
                script {
                    SECRET_FILE_PATH = credentials([file(credentialsId: 'appsettings_json')])
                }
                sh '''
                    docker build -t gitlantis/user-test-api-prod:latest -f Dockerfile .
                    cat $SECRET_FILE_PATH
                    docker run --rm -p 5000:5000 -p 80:80 -e ASPNETCORE_HTTP_PORT=http://+:5000 user-test-api-dev -v $SECRET_FILE_PATH:/App/appsettings.json
                '''
            }
        }        
    }
}