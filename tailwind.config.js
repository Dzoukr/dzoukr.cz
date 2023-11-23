import daisyui from 'daisyui'
import typography from '@tailwindcss/typography'
export default {
    content: [
        "./src/DzoukrCz.Client/.fable-build/**/*.{js,ts,jsx,tsx}",
    ],
    plugins: [
        daisyui,
        typography
    ]
}
