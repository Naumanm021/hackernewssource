import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HackerNewsService } from './hacker-news.service';
import { Story } from '../models/story.model';

describe('HackerNewsService', () => {
  let service: HackerNewsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [HackerNewsService]
    });
    service = TestBed.inject(HackerNewsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch newest stories', () => {
    const mockStories = [
      { id: 1, title: 'Test Story 1', url: 'http://test1.com', score: 100, by: 'user1', time: 1234567890, descendants: 10 },
      { id: 2, title: 'Test Story 2', url: 'http://test2.com', score: 200, by: 'user2', time: 1234567890, descendants: 20 }
    ];

    service.getNewestStories(2).subscribe(stories => {
      expect(stories.length).toBe(2);
      expect(stories[0].title).toBe('Test Story 1');
      expect(stories[1].title).toBe('Test Story 2');
    });

    const req = httpMock.expectOne('https://localhost:5001/api/hackernews/newest?count=2');
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);
  });

  it('should search stories', () => {
    const mockStories = [
      { id: 1, title: 'Angular is great', url: 'http://test1.com', score: 100, by: 'user1', time: 1234567890, descendants: 10 }
    ];

    service.searchStories('angular').subscribe(stories => {
      expect(stories.length).toBe(1);
      expect(stories[0].title).toBe('Angular is great');
    });

    const req = httpMock.expectOne('https://localhost:5001/api/hackernews/search?query=angular&count=10');
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);
  });
});
