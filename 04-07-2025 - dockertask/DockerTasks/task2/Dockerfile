#  Stage 1
# docker pull mysql:latest

FROM mysql:latest

RUN -d \
  --name my-mysql \
  -e MYSQL_ROOT_PASSWORD=password \
  -v mydbdata:/var/lib/mysql \
  -p 3306:3306 \
  mysql:latest

# docker exec -it my-mysql mysql -u root -p

RUN exec -it my-mysql mysql -u root -p

# Create a database
RUN mysql -u root -p -e "CREATE DATABASE testdb;"

# Create a table
RUN mysql -u root -p -e "USE testdb; CREATE TABLE users (id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(100), email VARCHAR(100),role VARCHAR(2));"

# Insert data into the table
RUN mysql -u root -p -e "USE testdb; INSERT INTO users (name, email, role) VALUES (101,'Poovarasan S','poovarasan@gmail.com','1');

# Select data from the table

RUN mysql -u root -p -e "USE testdb; SELECT * FROM users;"

# Database data is stored in the volume (mydbdata)

# Stop the container
RUN docker stop my-mysql

# Remove the container
RUN docker rm my-mysql

# Re-run the image mysql

RUN -d \
  --name my-mysql \
  -e MYSQL_ROOT_PASSWORD=password \
  -v mydbdata:/var/lib/mysql \
  -p 3306:3306 \
  mysql:latest

# Running with interactive terminal

RUN exec -it my-mysql mysql -u root -p

# Select data from the table
RUN mysql -u root -p -e "USE testdb; SELECT * FROM users;
# It will show the data in the table. it is mounted to the volume (mydbdata).
# Exit the MySQL shell
RUN exit
