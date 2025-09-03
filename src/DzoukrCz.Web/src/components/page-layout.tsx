import React from 'react'
import Link from 'next/link'

function TopMenuItem({ link, name, activeLink }: { link: string, name: string, activeLink: string }) {
    const isActive = link === activeLink ? "text-neutral bg-neutral border outline rounded-sm" : ""
    return (
        <Link href={link} className={`text-2xl text-warning py-2 px-4 not-a ${isActive}`}>{name}</Link>
    )
}

function TopMenu({ forPage } : { forPage: string }) {
    return (
        <nav className="navbar">
            <div className="navbar-start">
                <Link href="/" className="text-3xl font-bold text-white not-a">
                    &lt;džoukr<span className="text-warning">.cz/&gt;</span>
                </Link>
            </div>
            <div className="navbar-center"></div>
            <div className="navbar-end">
                <TopMenuItem activeLink={forPage} link="/" name="home" />
                <TopMenuItem activeLink={forPage} link="/speaking" name="speaking" />
            </div>
        </nav>
    )
}

function IconLink({ icon, label, href }: { icon: string; label: string; href: string }) {
  return (
    <div className="flex gap-1 items-center">
        <i className={icon} aria-hidden="true" />
        <Link href={href}>{label}</Link>
    </div>
  );
}

function Footer() {
  return (
    <div className="footer footer-vertical lg:footer-horizontal max-w-4xl mx-auto py-5 px-5 lg:px-0">
      <nav>
        <p>Roman &quot;Džoukr&quot; Provazník</p>
        <p className="text-xs opacity-90">Building software with ❤️ + 🧠</p>
      </nav>

      <nav>
        <div className="footer-title">Projects</div>
        <IconLink
          icon="fa-solid fa-podcast"
          label="PodVocasem"
          href="https://www.podvocasem.cz"
        />
        <IconLink
          icon="fa-solid fa-code"
          label="fsharpConf"
          href="https://fsharpconf.com"
        />
        <IconLink
          icon="fa-solid fa-hockey-puck"
          label="Hlášky"
          href="https://hlasky.dzoukr.cz"
        />
      </nav>

      <nav>
        <div className="footer-title">Contact</div>
        <IconLink
          icon="fa-regular fa-envelope"
          label="dzoukr@dzoukr.cz"
          href="mailto:dzoukr@dzoukr.cz"
        />
        <IconLink
          icon="fa-brands fa-bluesky"
          label="@dzoukr.cz"
          href="https://bsky.app/profile/dzoukr.cz"
        />
        <IconLink
          icon="fa-brands fa-github"
          label="dzoukr"
          href="https://github.com/dzoukr"
        />
        <IconLink
          icon="fa-brands fa-linkedin"
          label="Roman Provazník"
          href="https://www.linkedin.com/in/dzoukr/"
        />
      </nav>
    </div>
  );
}

export function PageLayout({forPage, children}: {forPage: string, children: React.ReactNode}) {
  return (
    <div className="grow max-w-4xl mx-auto py-3 px-3 lg:py-6 lg:px-0 flex flex-col gap-3 lg:gap-6">
      <TopMenu forPage={forPage} />
      <hr className="text-gray-700" />
      {children}
      <hr className="text-gray-700" />
      <Footer />
    </div>
  );
}