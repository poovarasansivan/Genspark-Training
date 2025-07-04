## Deploy a test service with rolling updates.

1. docker service create \
   --name webapp \
   --replicas 3 \
   --update-delay 10s \
   httpd

`overall progress: 3 out of 3 tasks 
1/3: running   [==================================================>] 
2/3: running   [==================================================>] 
3/3: running   [==================================================>] 
verify: Service mblenrs08v8isj8vuqa4o0nwd converged 
 `

2. docker service ls

`
ID             NAME        MODE         REPLICAS   IMAGE          PORTS
c77ht6dkoe4a   nginx-web   replicated   5/5        nginx:alpine   *:8080->80/tcp
mblenrs08v8i   webapp      replicated   3/3        httpd:latest   
`

3. docker service ps webapp

`
ID             NAME       IMAGE          NODE             DESIRED STATE   CURRENT STATE            ERROR     PORTS
l3t5tcg1b4l1   webapp.1   httpd:latest   docker-desktop   Running         Running 41 seconds ago             
ebvrjybesac8   webapp.2   httpd:latest   docker-desktop   Running         Running 41 seconds ago             
ytpzrsoesdp8   webapp.3   httpd:latest   docker-desktop   Running         Running 41 seconds ago       
`

4. docker service update \
  --image httpd:alpine \
  webapp

- Because of --update-delay 10s, Swarm will stop one replica and start the updated one then waits for 10 seconds and moves to next replica. It repeats the same process until all replica updates.

`
webapp
overall progress: 3 out of 3 tasks 
1/3: running   [==================================================>] 
2/3: running   [==================================================>] 
3/3: running   [==================================================>] 
verify: Service webapp converged 
`

5. docker service ps webapp

`
ID             NAME           IMAGE          NODE             DESIRED STATE   CURRENT STATE                 ERROR     PORTS
pt507g9489gi   webapp.1       httpd:alpine   docker-desktop   Running         Running about a minute ago              
l3t5tcg1b4l1    \_ webapp.1   httpd:latest   docker-desktop   Shutdown        Shutdown about a minute ago             
aibx1t6gp2br   webapp.2       httpd:alpine   docker-desktop   Running         Running about a minute ago              
ebvrjybesac8    \_ webapp.2   httpd:latest   docker-desktop   Shutdown        Shutdown about a minute ago             
qq67eco6uy4z   webapp.3       httpd:alpine   docker-desktop   Running         Running 52 seconds ago                  
ytpzrsoesdp8    \_ webapp.3   httpd:latest   docker-desktop   Shutdown        Shutdown 52 seconds ago     
`
