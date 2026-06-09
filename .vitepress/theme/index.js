import DefaultTheme from 'vitepress/theme'
import { h } from 'vue'
import './custom.css'

const GitHubLink = () =>
  h(
    'a',
    {
      class: 'rh-github-link',
      href: 'https://github.com/HPD-AI/Rhodium',
      target: '_blank',
      rel: 'noreferrer',
      'aria-label': 'Rhodium on GitHub',
      title: 'Rhodium on GitHub'
    },
    [
      h(
        'svg',
        {
          viewBox: '0 0 24 24',
          width: '20',
          height: '20',
          fill: 'currentColor',
          'aria-hidden': 'true'
        },
        [
          h('path', {
            d: 'M12 2C6.48 2 2 6.58 2 12.25c0 4.52 2.87 8.35 6.84 9.7.5.1.68-.22.68-.5 0-.24-.01-.88-.01-1.73-2.78.62-3.37-1.38-3.37-1.38-.46-1.18-1.11-1.5-1.11-1.5-.91-.64.07-.63.07-.63 1 .07 1.53 1.06 1.53 1.06.9 1.57 2.36 1.12 2.94.86.09-.66.35-1.12.63-1.38-2.22-.26-4.56-1.14-4.56-5.07 0-1.12.39-2.03 1.03-2.75-.1-.26-.45-1.31.1-2.72 0 0 .84-.28 2.75 1.05A9.3 9.3 0 0 1 12 6.92c.85 0 1.7.12 2.5.34 1.9-1.33 2.74-1.05 2.74-1.05.55 1.41.2 2.46.1 2.72.64.72 1.03 1.63 1.03 2.75 0 3.94-2.34 4.8-4.57 5.06.36.32.68.94.68 1.9 0 1.38-.01 2.49-.01 2.82 0 .28.18.6.69.5A10.08 10.08 0 0 0 22 12.25C22 6.58 17.52 2 12 2Z'
          })
        ]
      )
    ]
  )

export default {
  extends: DefaultTheme,
  Layout: () =>
    h(DefaultTheme.Layout, null, {
      'nav-bar-content-after': () => h(GitHubLink)
    })
}
