# Features
- Twitter authentication for voting
  - You can create a poll without authenticating youself, but you won't be able to manage it later
  - You can see polls without authenticating youself, but you won't be able to vote
- Sorting options (name/most voted)
- Poll shareable via id (random hash)
- Customization on creation page
  - Single/multi-vote
  - Show/hide voters
  - Poll duration
  - Allow users to add new options
- Poll management (author only)
  - End poll prematurely
  - Delete poll
  - Block users (and remove their votes if it's the case)
  - Unblock users (must vote again)
  - Delete options

# Front-end specs
- Minimalist design
- Localization support

# Back-end specs
- Should work as an independent API

# Database specs
- Physical deletion

## Entities
- poll(id*, multi_vote, show_voters, allow_add, end_date, user_id?)
- poll_block(id*, poll_id^, user_id)
- poll_option(id*, poll_id^, description)
- poll_vote(id*, option_id^, user_id)

---
`*`: pk  
`^`: fk cascade  
`?`: nullable
