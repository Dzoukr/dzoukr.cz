import { defineConfig } from 'vite'
import tailwindcss from "@tailwindcss/vite";

function customReloadPlugin() {
    return {
        name: 'custom-giraffe-reload',
        configureServer(server) {
            server.middlewares.use((req, res, next) => {
                if (req.url === '/__reload') {
                    server.ws.send({
                        type: 'full-reload',
                    })
                    res.end('reloaded')
                } else {
                    next()
                }
            })
        }
    };
};

/** @type {import('vite').UserConfig} */
export default defineConfig({
    root: './src/DzoukrCz.WebClient',
    build: {
        outDir: '../../publish/webApp/public',
        emptyOutDir: true,
        rollupOptions: {
            input: './src/DzoukrCz.WebClient/scripts.ts',
            output: {
                entryFileNames: 'scripts.js',
                assetFileNames: (assetInfo) => {
                    // Force the CSS output to be named styles.css
                    if (assetInfo.name && assetInfo.name.endsWith('.css')) {
                        return 'styles.css'
                    }
                    // Place all other assets in the root folder
                    return '[name][extname]'
                }
            }
        }
    },
    server: {
        port: 8080,
        strictPort: true,
        proxy: {
            '/api': 'http://localhost:5000'
        },
        watch: {
            ignored: [
                '**/*.fs' // Don't watch F# files directly
            ]
        }
    },
    plugins: [
        tailwindcss(),
        customReloadPlugin()
    ]
})
