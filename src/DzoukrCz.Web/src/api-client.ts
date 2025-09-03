export interface EventRecord {
    Date: string;  // ISO date string, e.g. "2018-03-29"
    Event: string;
    Title: string;
    Place: string; // e.g. "Prague, CZ"
    Lang: string;  // e.g. "CZ"
    Link: string;  // markdown link
    Logo: string;  // markdown image
}

export async function getEvents(): Promise<EventRecord[]> {
    const res = await fetch("https://media.dzoukr.cz/talks.json", { next: { revalidate: 3600 } }); 
    if (!res.ok) {
        throw new Error('Failed to fetch events');
    }
    return res.json();
}
