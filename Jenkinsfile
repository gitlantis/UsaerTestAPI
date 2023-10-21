pipeline {
    agent any
    options {
        buildDiscarder(logRotator(numToKeepStr: '5'))
    }
    environment {
        DOCKERHUB_CREDENTIALS = credentials('gitlantis-dockerhub')
        PWD = pwd()
        APPSETTINGS = "${PWD}/appsettings.json"
    }
    stages {
        stage('Push staging to dockerhub') {
            when {
                branch 'staging'               
            }
            steps {
                script {
                    withCredentials([file(credentialsId: 'appsettings.json', variable: 'SECRET_FILE_PATH')]) {
                        
                        sh '''
                            cp SECRET_FILE_PATH $PWD   
                            chmod 644 $APPSETTINGS
                            docker rm -f $(docker ps -aq)
                            docker rmi -f $(docker images -aq)
                            echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin
                            docker build -t gitlantis/user-test-api-dev:latest -f Dockerfile .
                            docker push gitlantis/user-test-api-dev:latest 
                            docker logout
                            docker run -d -p 80:8081 -e ASPNETCORE_HTTP_PORT=http://+:5000 gitlantis/user-test-api-dev:latest
                        '''
                    }
                }
            }
        }
        stage('Push main to dockerhub') {
            when {
               branch 'main'               
            }
            steps {
                script {
                    withCredentials([file(credentialsId: 'appsettings.json', variable: 'SECRET_FILE_PATH')]) {
                        sh '''
                            cp -f $SECRET_FILE_PATH $PWD
                            chmod 644 $APPSETTINGS
                            docker rm -f $(docker ps -aq)
                            docker rmi -f $(docker images -aq)
                            echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin
                            docker build -t gitlantis/user-test-api-prod:latest -f Dockerfile . 
                            docker push gitlantis/user-test-api-prod:latest 
                            docker logout
                            docker run -d -p 80:80 -e ASPNETCORE_HTTP_PORT=http://+:5000 gitlantis/user-test-api-prod:latest 
                        '''
                   }
                }
            }
        }        
    }
}