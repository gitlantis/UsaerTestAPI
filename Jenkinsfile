def RemoveContainers(){
    sh '''
        ANCESTOR_CKECK=$(docker ps -q --filter="name=$CONTAINER_NAME")  
        for N in $ANCESTOR_CKECK
        do
            docker stop $N
            docker rm $N
        done
    '''
    
}

def PushImage(){
    sh '''
        echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin
        docker build -t $CONTAINER -f Dockerfile .
        docker push $CONTAINER 
        docker logout
    '''
}

def RunContainer(){
    withCredentials([file(credentialsId: 'appsettings.json', variable: 'SECRET_FILE_PATH')]) {
        sh '''
           cp $SECRET_FILE_PATH $PWD   
           chmod 644 $APPSETTINGS
           docker run --name $CONTAINER_NAME -d -p $PORT:80 -e ASPNETCORE_HTTP_PORT=http://+:5000 $CONTAINER
           ANCESTOR_CKECK=$(docker ps -q --filter="name=$CONTAINER_NAME")
           docker cp $APPSETTINGS $ANCESTOR_CKECK:/App/appsettings.json
        '''
    }
}

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
                    env.CONTAINER_NAME="user-test-api-dev"
                    env.CONTAINER="gitlantis/"+CONTAINER_NAME+":latest"
                    env.DOCKERHUB_CREDENTIALS=DOCKERHUB_CREDENTIALS 
                    env.PWD=PWD
                    env.APPSETTINGS = APPSETTINGS
                    env.PORT=80

                    RemoveContainers()
                    PushImage()
                    RunContainer()
                }
            }
        }
        stage('Push main to dockerhub') {
            when {
               branch 'main'               
            }
            steps {
                script {
                    env.CONTAINER_NAME="user-test-api-prod"
                    env.CONTAINER="gitlantis/"+CONTAINER_NAME+":latest"
                    env.DOCKERHUB_CREDENTIALS=DOCKERHUB_CREDENTIALS
                    env.PWD=PWD
                    env.APPSETTINGS = APPSETTINGS
                    env.PORT=8081

                    RemoveContainers()
                    PushImage()
                    RunContainer()
                }
            }        
        }
    }
}