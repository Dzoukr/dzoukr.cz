import React from 'react'
import ReactMarkdown from 'react-markdown'
import {Prism as SyntaxHighlighter} from 'react-syntax-highlighter'
import {oneDark} from 'react-syntax-highlighter/dist/cjs/styles/prism'
import {oneLight} from 'react-syntax-highlighter/dist/cjs/styles/prism'

export const ReactMarkdownWithCode = ({markdown, ...props }) => {
    return (
        <ReactMarkdown
            children={markdown}
            components={{
                code({node, inline, className, children, ...props}) {
                    const match = /language-(\w+)/.exec(className || '')
                    return !inline && match ? (
                        <SyntaxHighlighter
                            {...props}
                            children={String(children).replace(/\n$/, '')}
                            showLineNumbers="true"
                            style={oneDark}
                            language={match[1]}
                            PreTag="div"
                        />
                    ) : (
                        <code {...props} className={className}>
                            {children}
                        </code>
                    )
                }
            }}
            {...props}
        />
        )
}
