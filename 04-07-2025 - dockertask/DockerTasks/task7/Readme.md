## Task: Update the Nginx service image to a newer version (nginx:alpine).

1. docker service update \
  --image nginx:alpine \
  nginx-web

`
overall progress: 3 out of 3 tasks 
1/3: running   [==================================================>] 
2/3: running   [==================================================>] 
3/3: running   [==================================================>] 
verify: Service nginx-web converged 
`

2. docker service ls

`
ID             NAME        MODE         REPLICAS   IMAGE          PORTS
c77ht6dkoe4a   nginx-web   replicated   3/3        nginx:alpine   *:8080->80/tcp
`

3. Verifying the service update without downtime - docker service ps nginx-web

`
ID             NAME          IMAGE          NODE             DESIRED STATE   CURRENT STATE           ERROR     PORTS
tv3sqp2pz5ck   nginx-web.1   nginx:alpine   docker-desktop   Running         Running 6 minutes ago             
wo0lty9gml6e   nginx-web.2   nginx:alpine   docker-desktop   Running         Running 6 minutes ago             
aaa6lh8edk6r   nginx-web.3   nginx:alpine   docker-desktop   Running         Running 6 minutes ago  
`