export interface EventRecord {
    Date: string;  
    Event: string;
    Title: string;
    Place: string; 
    Lang: string;  
    Link: string;  
    LinkActive: boolean;
    Logo: string;
}

function toSafeName<T>(items: any[]): T[] {
    const booleanKeys = Object.entries(items[0] || {})
        .filter(([_, value]) => typeof value === 'string' && ['true', 'false'].includes(value.toLowerCase()))
        .map(([key]) => key.split(/\s+/).join(''));

    return items.map(item => {
        const cleanedItem = Object.fromEntries(
            Object.entries(item).map(([key, value]) => {
                const cleanKey = key.split(/\s+/).join('');
                if (booleanKeys.includes(cleanKey)) {
                    return [cleanKey, String(value).toLowerCase() === 'true'];
                }
                return [cleanKey, value];
            })
        );
        return cleanedItem as T;
    });
}

export async function getEvents(): Promise<EventRecord[]> {
    const res = await fetch("https://moonserver.dzoukr.cz/data/talks", { next: { revalidate: 3600 } }); 
    if (!res.ok) {
        throw new Error('Failed to fetch events');
    }
    const json = res.json();
    return json.then(data => toSafeName<EventRecord>(data));
}


