import React, { useState, useEffect } from 'react';
import {
  LightBulbIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  ClockIcon,
  UserIcon,
  ChartBarIcon,
  SparklesIcon,
} from '@heroicons/react/24/outline';
import { Task, Project } from '../types';
import {
  aiService,
  TaskPrioritySuggestion,
  WorkloadAnalysis,
  DeadlineAnalysis,
} from '../services/aiService';
import { useAuthStore } from '../state/authStore';

interface AIInsightsProps {
  tasks: Task[];
  projects: Project[];
  onPriorityUpdate?: (
    taskId: string,
    priority: 'Low' | 'Medium' | 'High'
  ) => void;
}

export const AIInsights: React.FC<AIInsightsProps> = ({
  tasks,
  projects,
  onPriorityUpdate,
}) => {
  const [prioritySuggestions, setPrioritySuggestions] = useState<
    TaskPrioritySuggestion[]
  >([]);
  const [workloadAnalysis, setWorkloadAnalysis] =
    useState<WorkloadAnalysis | null>(null);
  const [deadlineAnalysis, setDeadlineAnalysis] = useState<DeadlineAnalysis[]>(
    []
  );
  const [taskSuggestions, setTaskSuggestions] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<
    'priorities' | 'workload' | 'deadlines' | 'suggestions'
  >('priorities');

  const { user } = useAuthStore();

  useEffect(() => {
    if (tasks.length > 0 && user) {
      loadAIInsights();
    }
  }, [tasks, user]);

  const loadAIInsights = async () => {
    if (!user) return;

    setLoading(true);
    try {
      const [suggestions, workload, deadlines, suggestions] = await Promise.all(
        [
          aiService.suggestTaskPriorities(tasks, projects),
          aiService.analyzeWorkload(user.id, tasks),
          aiService.analyzeDeadlines(tasks),
          aiService.generateTaskSuggestions(user.id, tasks, projects),
        ]
      );

      setPrioritySuggestions(suggestions);
      setWorkloadAnalysis(workload);
      setDeadlineAnalysis(deadlines);
      setTaskSuggestions(suggestions);
    } catch (error) {
      console.error('Error loading AI insights:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePriorityUpdate = (
    taskId: string,
    priority: 'Low' | 'Medium' | 'High'
  ) => {
    if (onPriorityUpdate) {
      onPriorityUpdate(taskId, priority);
    }
  };

  const getTaskById = (taskId: string) =>
    tasks.find((task) => task.id === taskId);

  const getRiskLevelColor = (riskLevel: string) => {
    switch (riskLevel) {
      case 'High':
        return 'text-red-600 bg-red-100';
      case 'Medium':
        return 'text-yellow-600 bg-yellow-100';
      case 'Low':
        return 'text-green-600 bg-green-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 80) return 'text-green-600';
    if (confidence >= 60) return 'text-yellow-600';
    return 'text-red-600';
  };

  if (loading) {
    return (
      <div className='card'>
        <div className='flex items-center space-x-2 mb-4'>
          <SparklesIcon className='h-5 w-5 text-blue-500' />
          <h3 className='text-lg font-medium text-gray-900'>AI Insights</h3>
        </div>
        <div className='animate-pulse space-y-3'>
          <div className='h-4 bg-gray-200 rounded w-3/4'></div>
          <div className='h-4 bg-gray-200 rounded w-1/2'></div>
          <div className='h-4 bg-gray-200 rounded w-2/3'></div>
        </div>
      </div>
    );
  }

  return (
    <div className='card'>
      <div className='flex items-center space-x-2 mb-4'>
        <SparklesIcon className='h-5 w-5 text-blue-500' />
        <h3 className='text-lg font-medium text-gray-900'>AI Insights</h3>
      </div>

      {/* Tab Navigation */}
      <div className='flex space-x-1 mb-4'>
        <button
          onClick={() => setActiveTab('priorities')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'priorities'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Priorities
        </button>
        <button
          onClick={() => setActiveTab('workload')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'workload'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Workload
        </button>
        <button
          onClick={() => setActiveTab('deadlines')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'deadlines'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Deadlines
        </button>
        <button
          onClick={() => setActiveTab('suggestions')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'suggestions'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Suggestions
        </button>
      </div>

      {/* Tab Content */}
      <div className='space-y-4'>
        {activeTab === 'priorities' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Priority Suggestions
            </h4>
            <div className='space-y-3'>
              {prioritySuggestions.slice(0, 5).map((suggestion) => {
                const task = getTaskById(suggestion.taskId);
                if (!task) return null;

                return (
                  <div
                    key={suggestion.taskId}
                    className='p-3 bg-gray-50 rounded-lg'
                  >
                    <div className='flex items-start justify-between'>
                      <div className='flex-1'>
                        <h5 className='text-sm font-medium text-gray-900'>
                          {task.title}
                        </h5>
                        <div className='flex items-center space-x-2 mt-1'>
                          <span
                            className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                              suggestion.suggestedPriority === 'High'
                                ? 'bg-red-100 text-red-800'
                                : suggestion.suggestedPriority === 'Medium'
                                ? 'bg-yellow-100 text-yellow-800'
                                : 'bg-gray-100 text-gray-800'
                            }`}
                          >
                            {suggestion.suggestedPriority}
                          </span>
                          <span
                            className={`text-xs ${getConfidenceColor(
                              suggestion.confidence
                            )}`}
                          >
                            {suggestion.confidence}% confidence
                          </span>
                        </div>
                        <div className='mt-2'>
                          {suggestion.reasoning.map((reason, index) => (
                            <p key={index} className='text-xs text-gray-600'>
                              • {reason}
                            </p>
                          ))}
                        </div>
                      </div>
                      {onPriorityUpdate && (
                        <button
                          onClick={() =>
                            handlePriorityUpdate(
                              suggestion.taskId,
                              suggestion.suggestedPriority
                            )
                          }
                          className='ml-2 px-2 py-1 text-xs bg-blue-500 text-white rounded hover:bg-blue-600'
                        >
                          Apply
                        </button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {activeTab === 'workload' && workloadAnalysis && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Workload Analysis
            </h4>
            <div className='space-y-4'>
              <div className='flex items-center justify-between'>
                <span className='text-sm text-gray-600'>Current Workload</span>
                <span className='text-sm font-medium'>
                  {workloadAnalysis.currentWorkload}%
                </span>
              </div>
              <div className='w-full bg-gray-200 rounded-full h-2'>
                <div
                  className={`h-2 rounded-full ${
                    workloadAnalysis.currentWorkload > 80
                      ? 'bg-red-500'
                      : workloadAnalysis.currentWorkload > 60
                      ? 'bg-yellow-500'
                      : 'bg-green-500'
                  }`}
                  style={{ width: `${workloadAnalysis.currentWorkload}%` }}
                ></div>
              </div>

              <div className='grid grid-cols-3 gap-4 text-center'>
                <div>
                  <div className='text-lg font-bold text-red-600'>
                    {workloadAnalysis.workloadDistribution.high}
                  </div>
                  <div className='text-xs text-gray-600'>High Priority</div>
                </div>
                <div>
                  <div className='text-lg font-bold text-yellow-600'>
                    {workloadAnalysis.workloadDistribution.medium}
                  </div>
                  <div className='text-xs text-gray-600'>Medium Priority</div>
                </div>
                <div>
                  <div className='text-lg font-bold text-green-600'>
                    {workloadAnalysis.workloadDistribution.low}
                  </div>
                  <div className='text-xs text-gray-600'>Low Priority</div>
                </div>
              </div>

              {workloadAnalysis.suggestedTasks.length > 0 && (
                <div>
                  <h5 className='text-sm font-medium text-gray-700 mb-2'>
                    Recommendations
                  </h5>
                  <div className='space-y-1'>
                    {workloadAnalysis.suggestedTasks.map(
                      (suggestion, index) => (
                        <p key={index} className='text-xs text-gray-600'>
                          • {suggestion}
                        </p>
                      )
                    )}
                  </div>
                </div>
              )}
            </div>
          </div>
        )}

        {activeTab === 'deadlines' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Deadline Analysis
            </h4>
            <div className='space-y-3'>
              {deadlineAnalysis
                .filter((analysis) => analysis.urgencyScore > 50)
                .slice(0, 5)
                .map((analysis) => {
                  const task = getTaskById(analysis.taskId);
                  if (!task) return null;

                  return (
                    <div
                      key={analysis.taskId}
                      className='p-3 bg-gray-50 rounded-lg'
                    >
                      <div className='flex items-start justify-between'>
                        <div className='flex-1'>
                          <h5 className='text-sm font-medium text-gray-900'>
                            {task.title}
                          </h5>
                          <div className='flex items-center space-x-2 mt-1'>
                            <span
                              className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getRiskLevelColor(
                                analysis.riskLevel
                              )}`}
                            >
                              {analysis.riskLevel} Risk
                            </span>
                            <span className='text-xs text-gray-600'>
                              {analysis.daysUntilDeadline <= 0
                                ? 'Overdue'
                                : `${analysis.daysUntilDeadline} days left`}
                            </span>
                          </div>
                          <div className='mt-2'>
                            {analysis.recommendations
                              .slice(0, 2)
                              .map((recommendation, index) => (
                                <p
                                  key={index}
                                  className='text-xs text-gray-600'
                                >
                                  • {recommendation}
                                </p>
                              ))}
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
            </div>
          </div>
        )}

        {activeTab === 'suggestions' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Smart Suggestions
            </h4>
            <div className='space-y-3'>
              {taskSuggestions.length > 0 ? (
                taskSuggestions.map((suggestion, index) => (
                  <div
                    key={index}
                    className='flex items-start space-x-2 p-3 bg-blue-50 rounded-lg'
                  >
                    <LightBulbIcon className='h-4 w-4 text-blue-500 mt-0.5 flex-shrink-0' />
                    <p className='text-sm text-blue-800'>{suggestion}</p>
                  </div>
                ))
              ) : (
                <div className='text-center py-4'>
                  <CheckCircleIcon className='h-8 w-8 text-green-500 mx-auto mb-2' />
                  <p className='text-sm text-gray-600'>
                    Great job! No immediate actions needed.
                  </p>
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
