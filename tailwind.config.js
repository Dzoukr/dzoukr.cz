module.exports = {
    // theme:{
    //     extend:{
    //         typography: {
    //             DEFAULT : {
    //                 css: {
    //                     pre: false,
    //                     'pre div': false,
    //                 }
    //             }
    //         }
    //     }
    // },
    content: [
        "./src/DzoukrCz.Client/.fable-build/**/*.{js,ts,jsx,tsx}",
    ],
    plugins: [
        require('daisyui'),
        require('@tailwindcss/typography')
    ]
}
