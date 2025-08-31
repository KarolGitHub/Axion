import { Task, Project } from '../types';

export interface TaskPrioritySuggestion {
  taskId: string;
  suggestedPriority: 'Low' | 'Medium' | 'High';
  confidence: number;
  reasoning: string[];
}

export interface WorkloadAnalysis {
  userId: string;
  currentWorkload: number; // 0-100
  suggestedTasks: string[];
  workloadDistribution: {
    high: number;
    medium: number;
    low: number;
  };
}

export interface DeadlineAnalysis {
  taskId: string;
  urgencyScore: number; // 0-100
  daysUntilDeadline: number;
  riskLevel: 'Low' | 'Medium' | 'High';
  recommendations: string[];
}

class AIService {
  // Simulate AI-powered task prioritization
  async suggestTaskPriorities(
    tasks: Task[],
    projects: Project[]
  ): Promise<TaskPrioritySuggestion[]> {
    // Simulate API delay
    await new Promise((resolve) => setTimeout(resolve, 1000));

    const suggestions: TaskPrioritySuggestion[] = [];

    for (const task of tasks) {
      const priority = this.calculatePriority(task, projects);
      const reasoning = this.generateReasoning(task, priority);

      suggestions.push({
        taskId: task.id,
        suggestedPriority: priority,
        confidence: this.calculateConfidence(task),
        reasoning,
      });
    }

    return suggestions;
  }

  // Analyze user workload
  async analyzeWorkload(
    userId: string,
    tasks: Task[]
  ): Promise<WorkloadAnalysis> {
    await new Promise((resolve) => setTimeout(resolve, 800));

    const userTasks = tasks.filter((task) => task.assignedToId === userId);
    const highPriorityTasks = userTasks.filter(
      (task) => task.priority === 'High'
    ).length;
    const mediumPriorityTasks = userTasks.filter(
      (task) => task.priority === 'Medium'
    ).length;
    const lowPriorityTasks = userTasks.filter(
      (task) => task.priority === 'Low'
    ).length;

    const totalTasks = userTasks.length;
    const currentWorkload = Math.min(
      100,
      totalTasks * 20 + highPriorityTasks * 15
    );

    return {
      userId,
      currentWorkload,
      suggestedTasks: this.suggestTaskReassignments(userTasks),
      workloadDistribution: {
        high: highPriorityTasks,
        medium: mediumPriorityTasks,
        low: lowPriorityTasks,
      },
    };
  }

  // Analyze deadline urgency
  async analyzeDeadlines(tasks: Task[]): Promise<DeadlineAnalysis[]> {
    await new Promise((resolve) => setTimeout(resolve, 600));

    return tasks.map((task) => {
      const daysUntilDeadline = task.dueDate
        ? Math.ceil(
            (new Date(task.dueDate).getTime() - new Date().getTime()) /
              (1000 * 60 * 60 * 24)
          )
        : 999;

      const urgencyScore = this.calculateUrgencyScore(
        daysUntilDeadline,
        task.priority
      );
      const riskLevel = this.determineRiskLevel(urgencyScore);

      return {
        taskId: task.id,
        urgencyScore,
        daysUntilDeadline,
        riskLevel,
        recommendations: this.generateDeadlineRecommendations(
          task,
          daysUntilDeadline
        ),
      };
    });
  }

  // Generate intelligent task suggestions
  async generateTaskSuggestions(
    userId: string,
    tasks: Task[],
    projects: Project[]
  ): Promise<string[]> {
    await new Promise((resolve) => setTimeout(resolve, 500));

    const userTasks = tasks.filter((task) => task.assignedToId === userId);
    const suggestions: string[] = [];

    // Check for overdue tasks
    const overdueTasks = userTasks.filter(
      (task) =>
        task.dueDate &&
        new Date(task.dueDate) < new Date() &&
        task.status !== 'Done'
    );
    if (overdueTasks.length > 0) {
      suggestions.push(
        `You have ${overdueTasks.length} overdue task(s). Consider updating their status or extending deadlines.`
      );
    }

    // Check for high priority tasks
    const highPriorityTasks = userTasks.filter(
      (task) => task.priority === 'High' && task.status !== 'Done'
    );
    if (highPriorityTasks.length > 3) {
      suggestions.push(
        'You have many high-priority tasks. Consider delegating some or adjusting priorities.'
      );
    }

    // Check for task distribution
    const todoTasks = userTasks.filter((task) => task.status === 'Todo').length;
    const inProgressTasks = userTasks.filter(
      (task) => task.status === 'In Progress'
    ).length;

    if (todoTasks > inProgressTasks * 2) {
      suggestions.push(
        'You have many tasks in "Todo" status. Consider starting work on some tasks to maintain momentum.'
      );
    }

    if (inProgressTasks > 5) {
      suggestions.push(
        'You have many tasks in progress. Consider focusing on completing a few before starting new ones.'
      );
    }

    return suggestions;
  }

  // Private helper methods
  private calculatePriority(
    task: Task,
    projects: Project[]
  ): 'Low' | 'Medium' | 'High' {
    let score = 0;
    const project = projects.find((p) => p.id === task.projectId);

    // Project priority influence
    if (project?.priority === 'High') score += 3;
    else if (project?.priority === 'Medium') score += 2;
    else score += 1;

    // Task status influence
    if (task.status === 'Todo') score += 1;
    else if (task.status === 'In Progress') score += 2;
    else if (task.status === 'Review') score += 3;

    // Deadline influence
    if (task.dueDate) {
      const daysUntilDeadline = Math.ceil(
        (new Date(task.dueDate).getTime() - new Date().getTime()) /
          (1000 * 60 * 60 * 24)
      );
      if (daysUntilDeadline <= 1) score += 5;
      else if (daysUntilDeadline <= 3) score += 4;
      else if (daysUntilDeadline <= 7) score += 3;
      else if (daysUntilDeadline <= 14) score += 2;
      else score += 1;
    }

    // Current priority influence
    if (task.priority === 'High') score += 2;
    else if (task.priority === 'Medium') score += 1;

    if (score >= 8) return 'High';
    if (score >= 5) return 'Medium';
    return 'Low';
  }

  private calculateConfidence(task: Task): number {
    let confidence = 70; // Base confidence

    // Higher confidence if task has clear deadline
    if (task.dueDate) confidence += 15;

    // Higher confidence if task is part of high-priority project
    if (task.priority === 'High') confidence += 10;

    // Lower confidence for new tasks
    if (task.status === 'Todo') confidence -= 5;

    return Math.min(100, Math.max(50, confidence));
  }

  private generateReasoning(
    task: Task,
    priority: 'Low' | 'Medium' | 'High'
  ): string[] {
    const reasoning: string[] = [];

    if (task.dueDate) {
      const daysUntilDeadline = Math.ceil(
        (new Date(task.dueDate).getTime() - new Date().getTime()) /
          (1000 * 60 * 60 * 24)
      );
      if (daysUntilDeadline <= 1) {
        reasoning.push('Deadline is within 24 hours');
      } else if (daysUntilDeadline <= 3) {
        reasoning.push('Deadline is within 3 days');
      } else if (daysUntilDeadline <= 7) {
        reasoning.push('Deadline is within a week');
      }
    }

    if (task.priority === 'High') {
      reasoning.push('Currently marked as high priority');
    }

    if (task.status === 'Review') {
      reasoning.push('Task is in review phase - near completion');
    }

    if (priority === 'High' && reasoning.length === 0) {
      reasoning.push('High impact task requiring immediate attention');
    }

    return reasoning;
  }

  private suggestTaskReassignments(tasks: Task[]): string[] {
    const suggestions: string[] = [];
    const highPriorityTasks = tasks.filter(
      (task) => task.priority === 'High' && task.status !== 'Done'
    );

    if (highPriorityTasks.length > 4) {
      suggestions.push(
        'Consider delegating some high-priority tasks to balance workload'
      );
    }

    const overdueTasks = tasks.filter(
      (task) =>
        task.dueDate &&
        new Date(task.dueDate) < new Date() &&
        task.status !== 'Done'
    );

    if (overdueTasks.length > 2) {
      suggestions.push(
        'Multiple overdue tasks detected - consider requesting deadline extensions'
      );
    }

    return suggestions;
  }

  private calculateUrgencyScore(
    daysUntilDeadline: number,
    priority: string
  ): number {
    let score = 0;

    // Deadline urgency
    if (daysUntilDeadline <= 0) score += 100; // Overdue
    else if (daysUntilDeadline <= 1) score += 90;
    else if (daysUntilDeadline <= 3) score += 80;
    else if (daysUntilDeadline <= 7) score += 60;
    else if (daysUntilDeadline <= 14) score += 40;
    else if (daysUntilDeadline <= 30) score += 20;
    else score += 10;

    // Priority influence
    if (priority === 'High') score += 20;
    else if (priority === 'Medium') score += 10;

    return Math.min(100, score);
  }

  private determineRiskLevel(urgencyScore: number): 'Low' | 'Medium' | 'High' {
    if (urgencyScore >= 80) return 'High';
    if (urgencyScore >= 50) return 'Medium';
    return 'Low';
  }

  private generateDeadlineRecommendations(
    task: Task,
    daysUntilDeadline: number
  ): string[] {
    const recommendations: string[] = [];

    if (daysUntilDeadline <= 0) {
      recommendations.push(
        'Task is overdue - update status or extend deadline'
      );
      recommendations.push('Consider breaking down into smaller subtasks');
    } else if (daysUntilDeadline <= 1) {
      recommendations.push('Deadline is tomorrow - prioritize this task');
      recommendations.push('Consider requesting help from team members');
    } else if (daysUntilDeadline <= 3) {
      recommendations.push('Deadline is approaching - focus on completion');
      recommendations.push(
        'Review task scope - consider reducing if necessary'
      );
    } else if (daysUntilDeadline <= 7) {
      recommendations.push('Plan your work schedule for this task');
      recommendations.push('Set intermediate milestones');
    }

    if (task.priority === 'High' && daysUntilDeadline <= 7) {
      recommendations.push(
        'High priority task with approaching deadline - requires immediate attention'
      );
    }

    return recommendations;
  }
}

export const aiService = new AIService();
