import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Story } from '../models/story.model';

@Injectable({
  providedIn: 'root'
})
export class HackerNewsService {
  // private apiUrl = 'https://localhost:7261/api/hackernews'; // Update with your API URL
  private apiUrl = 'https://hackernewsreaderapi20250514161518.azurewebsites.net/api/hackernews'; // Update with your API URL

  constructor(private http: HttpClient) { }

  getNewestStories(count: number = 10): Observable<Story[]> {
    return this.http.get<Story[]>(`${this.apiUrl}/newest?count=${count}`).pipe(
      map(stories => stories.map(story => this.mapToStory(story)))
    );
  }

  searchStories(query: string, count: number = 10): Observable<Story[]> {
    return this.http.get<Story[]>(`${this.apiUrl}/search?query=${query}&count=${count}`).pipe(
      map(stories => stories.map(story => this.mapToStory(story)))
    );
  }

  private mapToStory(data: any): Story {
    return {
      id: data.id,
      title: data.title,
      url: data.url,
      score: data.score,
      by: data.by,
      time: data.time * 1000, // Convert to milliseconds
      descendants: data.descendants
    };
  }
}
