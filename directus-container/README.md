# Directus CMS boilerplate

This is a boilerplate project for your directus installations. Just fork it!

## Requirements

Expect to update windows, WSL2 and Docker!

- Docker 3.3.3 (Engine 20.x) in WSL2 mode
  - WSL2 (Ubuntu or Debian)
    - Windows 10 1909
- sh (msys/gitbash, or actual)

## Setup

### Setup Once

1. Clone the repo
2. Set environment variables
   - Copy the default `<project-path>/sample.env` to `<project-path>/.env`
   - Make changes to `<project-path>/.env` to suit your needs (in particular your personal secrets)

With `sh` from inside `<project-path>`:

```bash
git clone git@repos.m-box.de:cms/directs-cms-boilerplate.git
cd directus-container
cp sample.env .env
```

Or use other tools of your choice for the same effect.

### Working with the container-stack

With `sh`/`cmd`/`ps` inside `<project-path>/directus-container`:

**Start** the container-stack

```bash
docker compose up -d
```

**Soft Stop/Restart** the container stack (without loosing state)

```bash
docker compose stop
```

```bash
docker compose restart
```

**Hard Stop** of the container stack (will loose all non-persistent state). This is required if the database should be re-initialized via `/db-init/init.sql`

```bash
docker compose down -v
```

**Revert** the container to the last database export. _CAUTION: Only works when used together with __Sharing Database Changes__ described below._

1. Remove the container stack
2. Start the container stack

### Repo push

When committing/pushing a change to `db-init/init.sql` or `cli-data/**` or `directus-data/**`, everyone else on the branch will also have to use that state! Make sure this is intended and consistent!

### Sharing Database Changes

With `sh` inside `<project-path>` while the container stack is up (shows green in Docker Desktop):

```sh
docker compose exec db pg_dump --user=directus --column-inserts --clean --if-exists --format=plain --dbname=directus > db-dump/dump_$(date +%d-%m-%y_%H%M%S).sql
```

On Windows just double-click on

```txt
./dump_db.bat
```

This dumps the database into a SQL-file at `<project-path>/db-dump/dump_<TIMESTAMP>.sql` to initialize the container (i.e. start the container) with that state, copy it to `<project-path>/db-init/init.sql`, overwriting any existing file.

```sh
cp ./db-dump/dump_<TIMESTAMP>.sql ./db-init/init.sql
```

Then just commit your changes. Dumps left in `<project-path>/db-dump` will not affect the container state.

## Admin UI

Access admin UI at [http://<hostname>:8055](http://localhost:8055) or form a different machine `http://<hostname>:8055`

Login: `admin@user.com`

## Trouble

Most known problems are solved by updating the required software (listed above) and restarting windows.

__PROBLEM__ docker crashes on startup on windows with some sort of "logon" failure
__FIX__ run `gpupdate /force` in with `cmd`, then use the container as normal.

__PROBLEM__ docker will not start because of some sort of "virtualization" failure
__PROBLEM__ WSL2 won't work
__FIX__ ensure virtualization is enabled in BIOS and as a system feature in Windows (google how to do that)

__PROBLEM__ `docker compose up -d` doesn't work because of "Duplicate mount point" or some such.
__FIX__ run `docker compose down` and/or `docker compose rm -fsv`, then use container as normal again.
