only 1 use cases are supported:
- get ticket type of branch

        uses: Raganhar/nup-github-action-jira-transition@main
        env:
          GITHUB_CONTEXT: "${{ toJson(github) }}"
        with:
          jira-api-key: ${{ secrets.JIRA_API_TOKEN }}
          jira-url: ${{ secrets.JIRA_BASE_URL }}
          jira-user: ${{ secrets.JIRA_USER_EMAIL }}
          main-jira-transition: done
          release-jira-transition: in progress


