import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StoryItemComponent } from './story-item.component';
import { Story } from '../../models/story.model';
import { MatCardModule } from '@angular/material/card';

describe('StoryItemComponent', () => {
  let component: StoryItemComponent;
  let fixture: ComponentFixture<StoryItemComponent>;

  const mockStory: Story = {
    id: 1,
    title: 'Test Story',
    url: 'http://example.com',
    score: 100,
    by: 'testuser',
    time: Date.now() - 3600000, // 1 hour ago
    descendants: 10
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StoryItemComponent],
      imports: [MatCardModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StoryItemComponent);
    component = fixture.componentInstance;
    component.story = mockStory;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display story title', () => {
    const titleElement = fixture.nativeElement.querySelector('mat-card-title');
    expect(titleElement.textContent).toContain(mockStory.title);
  });

  it('should extract domain from URL', () => {
    expect(component.domain).toBe('example.com');
  });

  it('should return correct time ago', () => {
    expect(component.timeAgo).toBe('1 hour ago');
  });
});
