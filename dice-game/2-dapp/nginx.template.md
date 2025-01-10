# 1. Prepare
```bash
brew install nginx

# turn to the directory of nginx
mkdir https
openssl req -nodes -new -x509 -keyout server.key -out server.crt
```

# 2. Config
```nginx
server {
    listen 443 ssl;
    server_name localhost;

    ssl_certificate     /opt/homebrew/etc/nginx/https/server.crt;
    ssl_certificate_key /opt/homebrew/etc/nginx/https/server.key;

    # template
    location / {
        proxy_pass http://localhost:3005;
        proxy_ssl_verify off;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # https://github.com/AElfProject/backend-templates
    location /v1/token {
        proxy_pass https://localhost:44376/token;
        proxy_ssl_verify off;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # https://github.com/AElfProject/backend-templates
    location /v1/api {
        proxy_set_header host $host;
        proxy_set_header X-real-ip $remote_addr;
        proxy_set_header X-forward-for $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_ssl_verify off;
        proxy_pass https://localhost:44376/api;
    }
    
    # https://github.com/AElfProject/aelf-observability-setup
    location /v1/traces {
        proxy_set_header host $host;
        proxy_set_header X-real-ip $remote_addr;
        proxy_set_header X-forward-for $proxy_add_x_forwarded_for;
        proxy_pass http://127.0.0.1:4316/v1/traces;
    }

    # packages/server-socket-io
    location /socket-io/ {
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header Host $host;

        proxy_pass http://localhost:3006;

        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
    }

    # packages/server-chaingpt
    location /api/chaingpt {
        proxy_set_header host $host;
        proxy_set_header X-real-ip $remote_addr;
        proxy_set_header X-forward-for $proxy_add_x_forwarded_for;
        proxy_pass http://127.0.0.1:3007/chaingpt;
    }
}
```

# 3. Run
```bash
nginx -t
nginx -s reload
```
