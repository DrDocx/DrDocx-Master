# DrDocx Master

This is the repository that contains all of the projects that make up the DrDocx codebase. It is set up this way because several of these projects are dependencies for other components of the product. We manage all this using ``git-subrepo``, so there's some setup required so you don't mess everything up.

## Setup

### Installing git-subrepo

I recommend you read up on [``git-subrepo``](https://github.com/ingydotnet/git-subrepo) briefly at this point. It's pretty straightforward but it'll help going forward. Here's an abbreviated installation guide pulled from their readme:

>``git clone https://github.com/ingydotnet/git-subrepo /path/to/git-subrepo``
``echo 'source /path/to/git-subrepo/.rc' >> ~/.bashrc``

NOTE: The path you use should be absolute or relative to your user directory or subsequent shells won't be able to find where you've put git-subrepo.

Make sure you replace that path with wherever you actually want to clone it, then use that path in the echo command.  Once you do that, restart your cmd/bash window, and typing ``git subrepo`` should yield something.

### Setting up DrDocx-Master

To clone this repository, simply run this command in the directory you want to put all your DrDocx repos:

```
git clone https://github.com/DrDocx/DrDocx-Master
```

git-subrepo is great and handles pretty much everything for us. We can make changes on the actual subrepos and then pull them into the master repo, or we can make changes in the subrepo directory on the master repo and then push it to the actual subrepo using just a command. Here's a couple examples:

- When you want to push the changes you've made on ``DrDocx-Master`` to ``DrDocx-Models`` for example, do the following:

```
git commit -m "[DrDocx-Master] You should prefix all your commits like that so we know where they came from when we push everything around"
git subrepo push DrDocx-Models
```

NOTE: Subrepo pushing takes a long time for some reason, it's (probably) not frozen if it sits there for a while.

Obviously, you don't need to do the first command if you've already committed your most recent changes. What the subrepo push command will do is push all the changes you've made in the DrDocx-Models that represents the DrDocx-Models subrepo to the remote DrDocx-Models repo.

- To pull in changes you made directly on a subrepo (e.g. making a change on ``DrDocx-Models`` directly on the repo instead of through ``DrDocx-Master``), just do the following once those changes are pushed to GitHub:

```
git subrepo pull DrDocx-Models
```

And that should be it. There's of course a lot more useful features and things to know about ``git-subrepo``, but that should be all you need to know to get moving. For more info, check out their [README](https://github.com/ingydotnet/git-subrepo/blob/master/ReadMe.pod).
