# Features
- Telegram authentication for voting ([docs](https://core.telegram.org/widgets/login))
  - You can create a poll without authenticating youself
  - You can see polls without authenticating youself, but you won't be able to vote
- Sorting options (name/most voted)
- Poll shareable via id (random hash)
- Customization on creation page
  - Single/multi-vote
  - Show/hide voters
  - Poll duration
  - Allow users to add new options
  
# Hosting specs
- Probably AWS Lightsail for starter
- Docker alpine everything

# Back-end specs
- Should work as an independent API

# Database specs
- PostgreSQL
- Physical deletion

## Entities
- user(id*, first_name, last_name, username, photo_url)
- poll(id*, multi_vote, show_voters, allow_add, end_date)
- poll_option(id*, poll_id^, description)
- poll_vote(option_id*^, user_id*^)

---
`*`: pk  
`^`: fk cascade  
`?`: nullable
