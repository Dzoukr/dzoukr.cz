import { PageLayout } from "@/components/page-layout";
import Link from "next/link";
import Image from "next/image";

function ContactBox({icon, title, linkUrl, linkTitle}: {icon: string, title: string, linkUrl:string, linkTitle:string}) {
  return (
    <div className="card card-border w-full shadow-xl bg-base-200">
      <div className="card-body">
        <div className="text-2xl flex gap-2 items-center mb-4">
          <i className={icon}/>
          <span>{title}</span>
        </div>
        <div className="text-lg">
          <Link href={linkUrl} target="_blank">{linkTitle}</Link>
        </div>
      </div>
    </div>
  );
}

export default function Contacts() {
  return (
    <PageLayout forPage="/contacts">
          <div className="formatted-text">
            <h1>Contacts</h1>
            <p>Here is the list of networks where we can connect. I'd love to get in touch, so don't hesitate to contact me.</p>
          </div>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
            <ContactBox icon="fa-regular fa-envelope" title="Email" linkUrl="mailto:dzoukr@dzoukr.cz" linkTitle="dzoukr@dzoukr.cz" />
            <ContactBox icon="fa-brands fa-bluesky" title="Bluesky" linkUrl="https://bsky.app/profile/dzoukr.cz" linkTitle="bsky.app/profile/dzoukr.cz" />
            <ContactBox icon="fa-brands fa-github" title="Github" linkUrl="https://github.com/dzoukr" linkTitle="github.com/dzoukr" />
            <ContactBox icon="fa-brands fa-linkedin" title="LinkedIn" linkUrl="https://www.linkedin.com/in/dzoukr/" linkTitle="linkedin.com/in/dzoukr/" />
          </div>
        </PageLayout>
  );
}
