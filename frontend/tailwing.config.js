/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                brand: {
                    black: '#0B101B',
                    grey: '#1E1E20',
                    blue: '#144EE3',
                    pink: '#EB568E',
                    lite: '#C9CED6',
                }
            },
            backgroundImage: {
                'brand-gradient': 'linear-gradient(90deg, #EB568E 0%, #144EE3 100%)',
            }
        },
    },
    plugins: [],
}