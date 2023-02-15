docker build --file .\BlazorAuthSpike.BlazorApp1\Dockerfile --tag eassbhhtgu/blazor-auth-spike:latest .
vdocker stack deploy --compose-file .\docker-compose.yml blazor-auth-spike
