'use client'

import React from 'react'
import Link from 'next/link'


function TopMenuItem({ link, name, activeLink }: { link: string, name: string, activeLink: string }) {
    const isActive = link === activeLink ? "text-neutral bg-neutral border outline rounded-sm" : ""
    return (
        <Link href={link} className={`text-2xl text-warning py-2 px-4 not-a ${isActive}`}>{name}</Link>
    )
}

function TopMenuMobileItem({ link, name, activeLink }: { link: string, name: string, activeLink: string }) {
    const isActive = link === activeLink ? "text-neutral bg-neutral border outline rounded-sm" : ""
    return (
        <li>
            <Link href={link} className={`text-xl text-warning py-2 px-4 not-a ${isActive}`}>{name}</Link>
        </li>
    )
}


export function TopMenu({ forPage }: { forPage: string }) {
    const [isMenuOpen, setIsMenuOpen] = React.useState(false);
    return (
        <>
            <nav className="navbar">
                <div className="navbar-start">
                    <Link href="/" className="text-3xl font-bold text-white not-a">
                        &lt;džoukr<span className="text-warning">.cz/&gt;</span>
                    </Link>
                </div>
                <div className="navbar-center"></div>
                <div className="navbar-end">
                    <div className="hidden sm:flex">
                        <TopMenuItem activeLink={forPage} link="/" name="home" />
                        <TopMenuItem activeLink={forPage} link="/speaking" name="speaking" />
                        <TopMenuItem activeLink={forPage} link="/contacts" name="contacts" />
                    </div>
                    <div className="flex sm:hidden p-2 text-warning bg-neutral outline rounded-sm" onClick={() => setIsMenuOpen(!isMenuOpen)}>
                        <i className="fa-solid fa-bars" />
                    </div>
                </div>
            </nav>
            {isMenuOpen && (
                <div className="flex sm:hidden">
                    <ul className="menu w-full">
                    <TopMenuMobileItem activeLink={forPage} link="/" name="home" />
                    <TopMenuMobileItem activeLink={forPage} link="/speaking" name="speaking" />
                    <TopMenuMobileItem activeLink={forPage} link="/contacts" name="contacts" />
                    </ul>
                </div>
            )}
        </>
    )
}