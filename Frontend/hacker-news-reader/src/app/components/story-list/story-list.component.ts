import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HackerNewsService } from '../../services/hacker-news.service';
import { Story } from '../../models/story.model';
import { PageEvent, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { StoryItemComponent } from '../story-item/story-item.component';
import { MatButtonModule } from '@angular/material/button';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-story-list',
  standalone: true,
  imports: [
    CommonModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatCardModule,
    StoryItemComponent,
    MatButtonModule,
    MatError
  ],
  templateUrl: './story-list.component.html',
  styleUrls: ['./story-list.component.scss']
})
export class StoryListComponent implements OnInit {
  @Input() searchQuery = '';
  stories: Story[] = [];
  isLoading = false;
  error: string | null = null;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20, 50];
  totalStories = 500;

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit(): void {
    this.loadStories();
  }

  ngOnChanges(): void {
    if (this.searchQuery) {
      this.searchStories();
    } else {
      this.loadStories();
    }
  }

  loadStories(): void {
    this.isLoading = true;
    this.error = null;

    this.hackerNewsService.getNewestStories(this.pageSize)
      .subscribe({
        next: (stories) => {
          this.stories = stories;
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Failed to load stories. Please try again later.';
          this.isLoading = false;
          console.error('Error loading stories:', err);
        }
      });
  }

  searchStories(): void {
    this.isLoading = true;
    this.error = null;

    this.hackerNewsService.searchStories(this.searchQuery, this.pageSize)
      .subscribe({
        next: (stories) => {
          this.stories = stories;
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Failed to search stories. Please try again later.';
          this.isLoading = false;
          console.error('Error searching stories:', err);
        }
      });
  }

  handlePageEvent(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    if (this.searchQuery) {
      this.searchStories();
    } else {
      this.loadStories();
    }
  }
}
