
# 검색인프라

검색 API를 제외한, 로컬에 필요한 의존 인프라(DB,ELK)등을 자동 구성합니다.

개발환경에 필요한 최소 데이터가 자동 인입처리됩니다. (init/firstsql.mysql)


## Local 실행

    docker-compose up -d : 뛰우기 d옵션을 빼면(종료:ctrl+c) 도커 작동콘솔을 볼수있다.
    docker-compose down : 중지


## Server 목록
- Mysql : localhost:13306 / searchdb:3306
- Adminer : http://localhost:13307/   (server:searchdb ,id:root,pw:root)
- ElasticSearch : http://localhost:9200/
- Kibana : http://localhost:5601/app/kibana
