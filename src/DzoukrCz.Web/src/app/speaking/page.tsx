import Link from "next/link";

import { getEvents, EventRecord } from "@/api-client";
import { PageLayout } from "@/components/page-layout";

function EventBox(event:EventRecord) {
  
  const date = new Intl.DateTimeFormat('cs-CZ', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).format(new Date(event.Date));
  
  
  return (
    <div className="card card-border w-full shadow-xl bg-base-200 ">
      {event.Logo && (
        
        <figure className="h-48 bg-white relative w-full overflow-hidden">
          <img 
        src={event.Logo}
        alt={event.Event}
        
        className="absolute inset-0 h-full w-full object-cover object-center"
          />
        </figure>
        
      )}
      <div className="card-body">
        <h2 className="card-title">{event.Title}</h2>
        <div className="text-lg mb-4">{event.Event}</div>
        
        <div className="flex gap-2">
          <div className="badge badge-primary">{event.Lang}</div>
        </div>
        
        <div className="flex gap-2 items-center">
          <i className="fa-regular fa-calendar opacity-80" />
          {date}
        </div>        
        
        <div className="flex gap-2 items-center">
          <i className="fa-solid fa-location-dot opacity-80" />
          {event.Place}
        </div>        
        
        
        <div className="card-actions justify-center mt-4 grow flex flex-row justify-center items-end">
          {event.Link && event.LinkActive && (
          <Link 
            href={event.Link} 
            className="btn btn-warning rounded-sm"
            target="_blank"
          >
            <i className="fa-solid fa-arrow-up-right-from-square" />
            Event detail
          </Link>
          )}
        </div>
      </div>
    </div>
  );
} 

export default async function Speaking() {
  const events = await getEvents();

  return (
    <PageLayout forPage="/speaking">
      <div className="formatted-text">
        <h1>Speaking</h1>
        <p>Here you can find a list of my talks and events I have attended as a speaker. If you want me to speak at your event, please don't hesitate to&nbsp;
          <Link href="/contacts">contact me</Link>.</p>
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {events.reverse().map((event, index) => (
          <EventBox key={index} {...event} />
        ))}
      </div>
    </PageLayout>
  );
}
