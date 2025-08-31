import { api } from '../utils/api';

// Types for third-party integrations
export interface ThirdPartyIntegration {
  id: string;
  name: string;
  provider: string;
  isEnabled: boolean;
  autoSync: boolean;
  syncSettings?: string;
  organizationId?: string;
  createdById?: string;
  createdByName?: string;
  createdAt: string;
  updatedAt?: string;
  lastSyncAt?: string;
  lastSyncStatus?: string;
  lastSyncError?: string;
}

export interface CreateThirdPartyIntegrationRequest {
  name: string;
  provider: string;
  clientId: string;
  clientSecret: string;
  webhookUrl?: string;
  webhookSecret?: string;
  autoSync: boolean;
  syncSettings?: string;
  organizationId?: string;
}

export interface UpdateThirdPartyIntegrationRequest {
  name: string;
  clientId: string;
  clientSecret: string;
  webhookUrl?: string;
  webhookSecret?: string;
  isEnabled: boolean;
  autoSync: boolean;
  syncSettings?: string;
}

export interface IntegrationEvent {
  id: string;
  integrationId: string;
  eventType: string;
  payload: string;
  externalId?: string;
  status?: string;
  errorMessage?: string;
  retryCount: number;
  processedAt?: string;
  createdAt: string;
}

export interface IntegrationSync {
  id: string;
  integrationId: string;
  syncType: string;
  entityType: string;
  entityId?: string;
  itemsProcessed: number;
  itemsCreated: number;
  itemsUpdated: number;
  itemsFailed: number;
  status?: string;
  errorMessage?: string;
  startedAt: string;
  completedAt?: string;
  durationMs: number;
}

export interface TestIntegrationRequest {
  integrationId: string;
  testType: string; // connection, webhook, sync
}

export interface TestIntegrationResponse {
  success: boolean;
  message?: string;
  errorDetails?: string;
  testedAt: string;
}

export interface IntegrationStats {
  integrationId: string;
  provider: string;
  totalEvents: number;
  pendingEvents: number;
  failedEvents: number;
  totalSyncs: number;
  successfulSyncs: number;
  failedSyncs: number;
  lastSuccessfulSync?: string;
  lastFailedSync?: string;
  lastError?: string;
}

export interface SyncIntegrationRequest {
  integrationId: string;
  syncType: string; // full, incremental
  entityType?: string; // project, task, user, etc.
  entityId?: string;
}

// GitHub types
export interface GitHubRepository {
  id: string;
  integrationId: string;
  externalId: string;
  name: string;
  fullName?: string;
  description?: string;
  url?: string;
  defaultBranch?: string;
  isPrivate: boolean;
  isFork: boolean;
  language?: string;
  stars: number;
  forks: number;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  issueCount: number;
  pullRequestCount: number;
}

export interface GitHubIssue {
  id: string;
  repositoryId: string;
  externalId: string;
  title: string;
  description?: string;
  state?: string;
  assignee?: string;
  author?: string;
  labels?: string;
  number: number;
  url?: string;
  createdAt?: string;
  updatedAt?: string;
  closedAt?: string;
  lastSyncAt: string;
}

export interface GitHubPullRequest {
  id: string;
  repositoryId: string;
  externalId: string;
  title: string;
  description?: string;
  state?: string;
  author?: string;
  assignee?: string;
  reviewers?: string;
  labels?: string;
  number: number;
  url?: string;
  baseBranch?: string;
  headBranch?: string;
  createdAt?: string;
  updatedAt?: string;
  mergedAt?: string;
  closedAt?: string;
  lastSyncAt: string;
}

// Slack types
export interface SlackWorkspace {
  id: string;
  integrationId: string;
  externalId: string;
  name: string;
  domain?: string;
  icon?: string;
  isEnterprise: boolean;
  enterpriseId?: string;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  channelCount: number;
  userCount: number;
}

export interface SlackChannel {
  id: string;
  workspaceId: string;
  externalId: string;
  name: string;
  topic?: string;
  purpose?: string;
  isPrivate: boolean;
  isArchived: boolean;
  memberCount: number;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  messageCount: number;
}

export interface SlackUser {
  id: string;
  workspaceId: string;
  externalId: string;
  name: string;
  realName?: string;
  email?: string;
  avatar?: string;
  status?: string;
  statusText?: string;
  isBot: boolean;
  isDeleted: boolean;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface SlackMessage {
  id: string;
  channelId: string;
  externalId: string;
  text: string;
  userId?: string;
  username?: string;
  type?: string;
  subtype?: string;
  threadTs?: string;
  parentMessageId?: string;
  attachments?: string;
  reactions?: string;
  timestamp?: string;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  replyCount: number;
}

// Google Workspace types
export interface GoogleWorkspaceDomain {
  id: string;
  integrationId: string;
  domain: string;
  name?: string;
  isPrimary: boolean;
  isVerified: boolean;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  userCount: number;
  groupCount: number;
}

export interface GoogleWorkspaceUser {
  id: string;
  domainId: string;
  externalId: string;
  email: string;
  firstName?: string;
  lastName?: string;
  displayName?: string;
  avatar?: string;
  department?: string;
  jobTitle?: string;
  location?: string;
  isAdmin: boolean;
  isSuspended: boolean;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
  groupCount: number;
}

export interface GoogleWorkspaceGroup {
  id: string;
  domainId: string;
  externalId: string;
  name: string;
  email?: string;
  description?: string;
  type?: string;
  memberCount: number;
  lastSyncAt?: string;
  createdAt: string;
  updatedAt?: string;
}

// Webhook types
export interface WebhookPayload {
  eventType: string;
  provider: string;
  externalId: string;
  payload: string;
  timestamp: string;
  signature?: string;
}

class ThirdPartyIntegrationService {
  // General integration management
  async getIntegrations(
    organizationId?: string
  ): Promise<ThirdPartyIntegration[]> {
    const params = organizationId ? `?organizationId=${organizationId}` : '';
    return api.get(`/integrations${params}`);
  }

  async getIntegration(integrationId: string): Promise<ThirdPartyIntegration> {
    return api.get(`/integrations/${integrationId}`);
  }

  async createIntegration(
    request: CreateThirdPartyIntegrationRequest
  ): Promise<ThirdPartyIntegration> {
    return api.post('/integrations', request);
  }

  async updateIntegration(
    integrationId: string,
    request: UpdateThirdPartyIntegrationRequest
  ): Promise<ThirdPartyIntegration> {
    return api.put(`/integrations/${integrationId}`, request);
  }

  async deleteIntegration(integrationId: string): Promise<boolean> {
    return api.delete(`/integrations/${integrationId}`);
  }

  async testIntegration(
    integrationId: string,
    testType: string
  ): Promise<TestIntegrationResponse> {
    return api.post(`/integrations/${integrationId}/test`, { testType });
  }

  async getIntegrationStats(integrationId: string): Promise<IntegrationStats> {
    return api.get(`/integrations/${integrationId}/stats`);
  }

  // Sync operations
  async syncIntegration(
    request: SyncIntegrationRequest
  ): Promise<IntegrationSync> {
    return api.post(`/integrations/${request.integrationId}/sync`, request);
  }

  async getIntegrationSyncs(integrationId: string): Promise<IntegrationSync[]> {
    return api.get(`/integrations/${integrationId}/syncs`);
  }

  async getIntegrationEvents(
    integrationId: string
  ): Promise<IntegrationEvent[]> {
    return api.get(`/integrations/${integrationId}/events`);
  }

  // GitHub operations
  async getGitHubRepositories(
    integrationId: string
  ): Promise<GitHubRepository[]> {
    return api.get(`/integrations/${integrationId}/github/repositories`);
  }

  async getGitHubRepository(repositoryId: string): Promise<GitHubRepository> {
    return api.get(`/integrations/github/repositories/${repositoryId}`);
  }

  async getGitHubIssues(repositoryId: string): Promise<GitHubIssue[]> {
    return api.get(`/integrations/github/repositories/${repositoryId}/issues`);
  }

  async getGitHubPullRequests(
    repositoryId: string
  ): Promise<GitHubPullRequest[]> {
    return api.get(
      `/integrations/github/repositories/${repositoryId}/pull-requests`
    );
  }

  async getGitHubIssue(issueId: string): Promise<GitHubIssue> {
    return api.get(`/integrations/github/issues/${issueId}`);
  }

  async getGitHubPullRequest(
    pullRequestId: string
  ): Promise<GitHubPullRequest> {
    return api.get(`/integrations/github/pull-requests/${pullRequestId}`);
  }

  // Slack operations
  async getSlackWorkspaces(integrationId: string): Promise<SlackWorkspace[]> {
    return api.get(`/integrations/${integrationId}/slack/workspaces`);
  }

  async getSlackWorkspace(workspaceId: string): Promise<SlackWorkspace> {
    return api.get(`/integrations/slack/workspaces/${workspaceId}`);
  }

  async getSlackChannels(workspaceId: string): Promise<SlackChannel[]> {
    return api.get(`/integrations/slack/workspaces/${workspaceId}/channels`);
  }

  async getSlackUsers(workspaceId: string): Promise<SlackUser[]> {
    return api.get(`/integrations/slack/workspaces/${workspaceId}/users`);
  }

  async getSlackMessages(channelId: string): Promise<SlackMessage[]> {
    return api.get(`/integrations/slack/channels/${channelId}/messages`);
  }

  // Google Workspace operations
  async getGoogleWorkspaceDomains(
    integrationId: string
  ): Promise<GoogleWorkspaceDomain[]> {
    return api.get(`/integrations/${integrationId}/google/domains`);
  }

  async getGoogleWorkspaceDomain(
    domainId: string
  ): Promise<GoogleWorkspaceDomain> {
    return api.get(`/integrations/google/domains/${domainId}`);
  }

  async getGoogleWorkspaceUsers(
    domainId: string
  ): Promise<GoogleWorkspaceUser[]> {
    return api.get(`/integrations/google/domains/${domainId}/users`);
  }

  async getGoogleWorkspaceGroups(
    domainId: string
  ): Promise<GoogleWorkspaceGroup[]> {
    return api.get(`/integrations/google/domains/${domainId}/groups`);
  }

  // Utility methods
  getProviderIcon(provider: string): string {
    switch (provider.toLowerCase()) {
      case 'github':
        return 'https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png';
      case 'slack':
        return 'https://a.slack-edge.com/bv1-10/slack_logo-ebd02d1.svg';
      case 'googleworkspace':
        return 'https://developers.google.com/workspace/images/logo-gws.svg';
      default:
        return '';
    }
  }

  getProviderColor(provider: string): string {
    switch (provider.toLowerCase()) {
      case 'github':
        return 'bg-gray-900';
      case 'slack':
        return 'bg-purple-600';
      case 'googleworkspace':
        return 'bg-blue-600';
      default:
        return 'bg-gray-600';
    }
  }

  formatSyncStatus(status?: string): { text: string; color: string } {
    switch (status) {
      case 'completed':
        return { text: 'Completed', color: 'text-green-600' };
      case 'running':
        return { text: 'Running', color: 'text-blue-600' };
      case 'failed':
        return { text: 'Failed', color: 'text-red-600' };
      case 'pending':
        return { text: 'Pending', color: 'text-yellow-600' };
      default:
        return { text: 'Unknown', color: 'text-gray-600' };
    }
  }

  formatEventStatus(status?: string): { text: string; color: string } {
    switch (status) {
      case 'processed':
        return { text: 'Processed', color: 'text-green-600' };
      case 'pending':
        return { text: 'Pending', color: 'text-yellow-600' };
      case 'failed':
        return { text: 'Failed', color: 'text-red-600' };
      default:
        return { text: 'Unknown', color: 'text-gray-600' };
    }
  }

  parseLabels(labels?: string): string[] {
    if (!labels) return [];
    try {
      return JSON.parse(labels);
    } catch {
      return [];
    }
  }

  parseAttachments(attachments?: string): any[] {
    if (!attachments) return [];
    try {
      return JSON.parse(attachments);
    } catch {
      return [];
    }
  }

  parseReactions(reactions?: string): any[] {
    if (!reactions) return [];
    try {
      return JSON.parse(reactions);
    } catch {
      return [];
    }
  }
}

export const thirdPartyIntegrationService = new ThirdPartyIntegrationService();
