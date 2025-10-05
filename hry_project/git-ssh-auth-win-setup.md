# Setup SSH Authentication for Git Bash on Windows

originaly posted by @bsara at https://gist.github.com/bsara/5c4d90db3016814a3d2fe38d314f9c23

## Prepararation

0. Create a folder at the root of your user home folder
   (Example: `C:/Users/uname/`) called `.ssh`.
1. Create the following files if they do not already
   exist (paths begin from the root of your user home
   folder):
    - `.ssh/config`
    - `.bash_profile`
    - `.bashrc`


## Create a New SSH Key

Follow the steps in the section named "Generating a new SSH
Key" found in the following documentation from GitHub:
*[Generating a new SSH key and adding it to the ssh-agent](https://help.github.com/articles/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent/#platform-windows)*


## Configure SSH for Git Hosting Server

Add the following text to `.ssh/config` (`.ssh` should be found
in the root of your user home folder):

```
Host github.com
 Hostname github.com
 IdentityFile ~/.ssh/id_rsa
```


## Enable SSH Agent Startup Whenever Git Bash is Started

First, ensure that following lines are added to `.bash_profile`,
which should be found in your root user home folder:

```sh
test -f ~/.profile && . ~/.profile
test -f ~/.bashrc && . ~/.bashrc
```

Now, add the following text to `.bashrc`, which should be found
in your root user home folder:

```sh
# Start SSH Agent
#----------------------------

SSH_ENV="$HOME/.ssh/environment"

function run_ssh_env {
  . "${SSH_ENV}" > /dev/null
}

function start_ssh_agent {
  echo "Initializing new SSH agent..."
  ssh-agent | sed 's/^echo/#echo/' > "${SSH_ENV}"
  echo "succeeded"
  chmod 600 "${SSH_ENV}"

  run_ssh_env;

  ssh-add ~/.ssh/id_rsa;
}

if [ -f "${SSH_ENV}" ]; then
  run_ssh_env;
  ps -ef | grep ${SSH_AGENT_PID} | grep ssh-agent$ > /dev/null || {
    start_ssh_agent;
  }
else
  start_ssh_agent;
fi
```