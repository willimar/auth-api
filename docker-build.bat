docker build -t authentic-api .

heroku login
heroku container:login

docker tag authentic-api registry.heroku.com/authentic-api/web
docker push registry.heroku.com/authentic-api/web

heroku container:release web -a authentic-api