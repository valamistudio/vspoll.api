![Build & Test](https://github.com/valamistudio/vspoll.api/workflows/Build%20&%20Test/badge.svg)
![Docker Build](https://github.com/valamistudio/vspoll.api/workflows/Docker%20build/badge.svg)

# Features
- Telegram authentication for voting
  - You can create a poll without authenticating youself
  - You can see polls without authenticating youself, but you won't be able to vote
- Sorting options (name/most voted)
- Poll shareable via id (random hash)
- Customization on creation page
  - Single/multi-vote
  - Show/hide voters
  - Poll duration
  - Allow/disallow users to add new options
  
# How to host
- Install docker (this application uses a Linux container)
- Follow the [telegram authentication setup](https://core.telegram.org/widgets/login)
- Setup an `.env` file following the `dev.env` example
- Run `docker-compose up -d`
- Alternatively, you can host or use an existing PostgreSQL 12.2 server and run the .NET Core API
