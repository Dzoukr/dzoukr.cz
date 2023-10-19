module.exports = {
    content: [
        "./src/DzoukrCz.Client/.fable-build/**/*.{js,ts,jsx,tsx}",
    ],
    plugins: [
        require('daisyui'),
        require('@tailwindcss/typography')
    ]
}
