# DrDocx Master

This is the repository that contains all of the projects that make up the DrDocx codebase. It is set up this way because several of these projects are dependencies for other components of the product. We manage all this using ``git-subrepo``, so there's some setup required so you don't mess everything up.

## Setup

### Installing git-subrepo

I recommend you read up on [``git-subrepo``](https://github.com/ingydotnet/git-subrepo) briefly at this point. It's pretty straightforward but it'll help going forward. Here's an abbreviated installation guide pulled from their readme:

>``git clone https://github.com/ingydotnet/git-subrepo /path/to/git-subrepo``

The next step depends on platform.
On Windows:
>``echo 'source /path/to/git-subrepo/.rc' >> ~/.bashrc``

On Mac:

>``make install``

NOTE: You may need to run this with ``sudo``/run cmd as an administrator to have permission to do this.

Make sure you replace that path with wherever you actually want to clone it.  Once you do that, restart your cmd/bash window, and typing ``git subrepo`` should yield something. If you encounter any installation issues, refer to the ``git-subrepo`` README linked above.

### Setting up DrDocx-Master

To clone this repository, simply run this command in the directory you want to put all your DrDocx repos:

```
git clone https://github.com/DrDocx/DrDocx-Master
```

### Using git-subrepo

git-subrepo is great and handles pretty much everything for us. We can make changes on the actual subrepos and then pull them into the master repo, or we can make changes in the subrepo directory on the master repo and then push it to the actual subrepo using just a command. Here's a couple examples:

- When you want to push the changes you've made on ``DrDocx-Master`` to ``DrDocx-Models`` for example, do the following:

```
git commit -m "[DrDocx-Master] You should prefix all your commits like that so we know where they came from when we push everything around"
git subrepo push DrDocx-Models
```
NOTE: Subrepo pushing takes a long time for some unknown reason, it's (probably) not frozen if it sits there for a while.

You may run into an issue where git tells you that the remote branch is already up to date. If so, run this command and try again:
```
git subrepo clean DrDocx-[project]
```

Obviously, you don't need to do the first command if you've already committed your most recent changes. What the subrepo push command will do is push all the changes you've made in the DrDocx-Models that represents the DrDocx-Models subrepo to the remote DrDocx-Models repo.

- To pull in changes you made directly on a subrepo (e.g. making a change on ``DrDocx-Models`` directly on the repo instead of through ``DrDocx-Master``), just do the following once those changes are pushed to GitHub:

```
git subrepo pull DrDocx-Models
```

And that should be it. There's of course a lot more useful features and things to know about ``git-subrepo``, but that should be all you need to know to get moving. For more info, check out their [README](https://github.com/ingydotnet/git-subrepo/blob/master/ReadMe.pod).
