
# Elastic Search를 활용하여 검색엔진 만들기 (.net core)

# 엘라스틱 서치 로컬 인프라 준비

[도커 인프라](/Infra/README.md)

# TypeGen (C#->.ts)

검색엔진에 사용된 모델을 프론트(TypeScript)로 자동 내려주기 위해 사용되었습니다.

    dotnet tool install --global dotnet-typegen --version 2.4.8

    dotnet typegen generate

# Elastic Search

    http://localhost:5601/app/kibana

Links:
- https://miroslavpopovic.com/posts/2018/07/elasticsearch-with-aspnet-core-and-docker
- https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-analyzers.html
- https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/testing-analyzers.html


# Docker Build


    docker build -t core-searchapi-app:dev ./SearchApi/

    docker run -e ASPNETCORE_ENVIRONMENT=Development --publish 5005:5000 --name core-searchapi core-searchapi-app

    http://localhost:5005/swagger/index.html

