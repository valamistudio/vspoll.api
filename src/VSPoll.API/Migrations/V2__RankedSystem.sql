alter table poll_votes add column weight integer not null default 1 check (weight > 0);
create type voting_system as enum ('SingleOption', 'MultiOption', 'Ranked');
alter table polls add column voting_system voting_system not null default 'SingleOption';
update polls set voting_system = 'SingleOption' where not multi_vote;
update polls set voting_system = 'MultiOption' where multi_vote;
alter table polls drop column multi_vote;
