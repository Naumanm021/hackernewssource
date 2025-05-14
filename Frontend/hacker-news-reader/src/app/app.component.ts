import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { StoryListComponent } from './components/story-list/story-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    SearchBarComponent,
    StoryListComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Hacker News Reader';
  searchQuery = '';

  onSearch(query: string): void {
    this.searchQuery = query;
  }
}
