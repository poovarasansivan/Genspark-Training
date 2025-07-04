## Docker SWARM

1. docker swarm init
   `Swarm initialized: current node (xljxd9ivrrcazjw2nhixgofrf) is now a manager.`

2. docker service create \
   --name nginx-web \
   --replicas 3 \
   --publish 8080:80 \
   nginx:alpine

`c77ht6dkoe4a1cjwfs2ybtu5x
overall progress: 3 out of 3 tasks 
1/3: running   [==================================================>] 
2/3: running   [==================================================>] 
3/3: running   [==================================================>] 
verify: Service c77ht6dkoe4a1cjwfs2ybtu5x converged 
`

3. docker service ls

`poovarasans@C02D8EFPMD6M-poovarasans task6 % docker service ls
ID             NAME        MODE         REPLICAS   IMAGE          PORTS
c77ht6dkoe4a   nginx-web   replicated   3/3        nginx:alpine   *:8080->80/tcp`

4. docker service ps nginx-web

`
ID             NAME          IMAGE          NODE             DESIRED STATE   CURRENT STATE                ERROR     PORTS
tv3sqp2pz5ck   nginx-web.1   nginx:alpine   docker-desktop   Running         Running about a minute ago             
wo0lty9gml6e   nginx-web.2   nginx:alpine   docker-desktop   Running         Running about a minute ago             
aaa6lh8edk6r   nginx-web.3   nginx:alpine   docker-desktop   Running         Running about a minute ago         
`
