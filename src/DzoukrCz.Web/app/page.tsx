import { PageLayout } from "@/components/page-layout";
import Link from "next/link";
import Image from "next/image";

export default function Home() {
  return (
    <PageLayout forPage="/">
      <div className="formatted-text">
        
        <Image src={"/profile.png"} alt="Roman" width={300} height={300} className="not-prose mx-auto lg:float-left mb-6 lg:my-0 lg:mr-8 w-[60%] lg:w-[30%] object-cover rounded-xl" />

        <h1>👋 Hi, I&apos;m Roman</h1>

        <p>
          I am a Head of Product Engineering at&nbsp;
          <a href="https://www.garwan.com">Garwan</a>, melomaniac,{" "}
          <Link href="/speaking">conference speaker</Link>,{" "}
          <a href="https://www.podvocasem.cz">podcaster</a>, and a terrible drummer.
        </p>

        <p>
          I have more than 20 years of experience with software development using languages like
          Pascal, Delphi, Prolog, PHP, C#, or F#, but I successfully managed to erase most of these
          languages from my brain.
        </p>

        <p>
          As a big fan of functional-first .NET language{" "}
          <a href="https://fsharp.org">F#</a>, I founded the Czech F# community called FSharping which
          I love to maintain and grow. I also somehow sneaked into F# Software Foundation Board of
          Trustees for one year.
        </p>

        <p>
          If you were listening to Kiss Radio between 2002 and 2005, I&apos;m the guy who annoyed you
          during the morning show.
        </p>

        <p>
          Since October 2021, I am co-hosting a Czech IT podcast called{" "}
          <a href="https://www.podvocasem.cz">PodVocasem</a>.
        </p>
      </div>
    </PageLayout>
  );
}
