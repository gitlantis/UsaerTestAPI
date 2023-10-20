pipeline {
    agent any
    options {
        buildDiscarder(logRotator(numToKeepStr: '5'))
    }
    environment {
        DOCKERHUB_CREDENTIALS = credentials('gitlantis-dockerhub')
    }
    stages {
        stage('Push staging to dockerhub') {
            when {
                branch 'staging'               
            }
            steps {
                script {
                    SECRET_FILE_PATH = credentials([file(credentialsId: 'appsettings.json')])
                }
                sh '''
                    echo $DOCKERHUB_CREDENTIALS_PSW 
                    echo $DOCKERHUB_CREDENTIALS_USR 
                    docker build -t gitlantis/user-test-api-dev:latest -f Dockerfile .
                    echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u DOCKERHUB_CREDENTIALS_USR --password-stdin
                    docker push gitlantis/user-test-api-dev:latest 
                    docker logout
                    cat $SECRET_FILE_PATH
                    docker run --rm -p 5000:5000 -p 80:8080 -e ASPNETCORE_HTTP_PORT=http://+:5000 user-test-api-dev -v $SECRET_FILE_PATH:/App/appsettings.json
                '''
            }
        }
        stage('Push main to dockerhub') {
            when {
               branch 'main'               
            }
            steps {
                script {
                    SECRET_FILE_PATH = credentials([file(credentialsId: 'appsettings.json')])
                }
                sh '''
                    echo $DOCKERHUB_CREDENTIALS_PSW 
                    echo $DOCKERHUB_CREDENTIALS_USR 
                    docker build -t gitlantis/user-test-api-prod:latest -f Dockerfile .
                    echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u DOCKERHUB_CREDENTIALS_USR --password-stdin
                    docker push gitlantis/user-test-api-prod:latest 
                    docker logout
                    cat $SECRET_FILE_PATH
                    docker run --rm -p 5000:5000 -p 80:8080 -e ASPNETCORE_HTTP_PORT=http://+:5000 user-test-api-prod -v $SECRET_FILE_PATH:/App/appsettings.json
                '''
            }
        }        
    }
}