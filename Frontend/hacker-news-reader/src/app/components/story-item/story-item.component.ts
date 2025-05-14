import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Story } from '../../models/story.model';

@Component({
  selector: 'app-story-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './story-item.component.html',
  styleUrls: ['./story-item.component.scss']
})
export class StoryItemComponent {
  @Input() story!: Story;
  @Input() index!: number;

  get domain(): string | null {
    if (!this.story.url) return null;
    try {
      const domain = new URL(this.story.url).hostname.replace('www.', '');
      return domain;
    } catch {
      return null;
    }
  }

  get timeAgo(): string {
    const seconds = Math.floor((Date.now() - this.story.time) / 1000);

    const intervals = {
      year: 31536000,
      month: 2592000,
      week: 604800,
      day: 86400,
      hour: 3600,
      minute: 60
    };

    for (const [unit, secondsInUnit] of Object.entries(intervals)) {
      const interval = Math.floor(seconds / secondsInUnit);
      if (interval >= 1) {
        return interval === 1 ? `${interval} ${unit} ago` : `${interval} ${unit}s ago`;
      }
    }

    return 'just now';
  }
}
