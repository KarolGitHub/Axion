import React, { useState } from 'react';
import { View, StyleSheet, ScrollView, RefreshControl } from 'react-native';
import { Text, Card, Button, useTheme, Surface } from 'react-native-paper';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

export const DashboardScreen: React.FC = () => {
  const [refreshing, setRefreshing] = useState(false);
  const theme = useTheme();

  const onRefresh = React.useCallback(() => {
    setRefreshing(true);
    // Simulate data refresh
    setTimeout(() => setRefreshing(false), 1000);
  }, []);

  const stats = [
    { title: 'Active Projects', value: '12', icon: 'folder', color: '#3b82f6' },
    {
      title: 'Tasks Due Today',
      value: '8',
      icon: 'checkmark-circle',
      color: '#ef4444',
    },
    {
      title: 'Resources Available',
      value: '15',
      icon: 'calendar',
      color: '#10b981',
    },
    { title: 'Team Members', value: '24', icon: 'people', color: '#8b5cf6' },
  ];

  const recentTasks = [
    {
      id: '1',
      title: 'Design Review',
      project: 'Website Redesign',
      status: 'In Progress',
    },
    {
      id: '2',
      title: 'API Integration',
      project: 'Mobile App',
      status: 'Review',
    },
    {
      id: '3',
      title: 'User Testing',
      project: 'E-commerce Platform',
      status: 'Todo',
    },
  ];

  return (
    <ScrollView
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
      }
    >
      <LinearGradient colors={['#2563eb', '#1d4ed8']} style={styles.header}>
        <Text style={styles.headerTitle}>Dashboard</Text>
        <Text style={styles.headerSubtitle}>Welcome back!</Text>
      </LinearGradient>

      <View style={styles.content}>
        {/* Stats Cards */}
        <View style={styles.statsContainer}>
          {stats.map((stat, index) => (
            <Card key={index} style={styles.statCard}>
              <Card.Content style={styles.statContent}>
                <Ionicons
                  name={stat.icon as any}
                  size={24}
                  color={stat.color}
                />
                <Text style={styles.statValue}>{stat.value}</Text>
                <Text style={styles.statTitle}>{stat.title}</Text>
              </Card.Content>
            </Card>
          ))}
        </View>

        {/* Recent Tasks */}
        <Card style={styles.section}>
          <Card.Title title='Recent Tasks' />
          <Card.Content>
            {recentTasks.map((task) => (
              <View key={task.id} style={styles.taskItem}>
                <View style={styles.taskInfo}>
                  <Text style={styles.taskTitle}>{task.title}</Text>
                  <Text style={styles.taskProject}>{task.project}</Text>
                </View>
                <View style={styles.taskStatus}>
                  <Text style={styles.statusText}>{task.status}</Text>
                </View>
              </View>
            ))}
          </Card.Content>
        </Card>

        {/* Quick Actions */}
        <Card style={styles.section}>
          <Card.Title title='Quick Actions' />
          <Card.Content>
            <View style={styles.actionButtons}>
              <Button
                mode='contained'
                icon='plus'
                style={styles.actionButton}
                onPress={() => {}}
              >
                New Task
              </Button>
              <Button
                mode='contained'
                icon='calendar'
                style={styles.actionButton}
                onPress={() => {}}
              >
                Book Resource
              </Button>
            </View>
          </Card.Content>
        </Card>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f3f4f6',
  },
  header: {
    paddingTop: 60,
    paddingBottom: 20,
    paddingHorizontal: 20,
  },
  headerTitle: {
    fontSize: 28,
    fontWeight: 'bold',
    color: 'white',
  },
  headerSubtitle: {
    fontSize: 16,
    color: 'rgba(255, 255, 255, 0.8)',
    marginTop: 4,
  },
  content: {
    padding: 16,
  },
  statsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: 16,
  },
  statCard: {
    width: '48%',
    marginBottom: 12,
  },
  statContent: {
    alignItems: 'center',
    padding: 16,
  },
  statValue: {
    fontSize: 24,
    fontWeight: 'bold',
    marginTop: 8,
  },
  statTitle: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 4,
    textAlign: 'center',
  },
  section: {
    marginBottom: 16,
  },
  taskItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: '#e5e7eb',
  },
  taskInfo: {
    flex: 1,
  },
  taskTitle: {
    fontSize: 16,
    fontWeight: '500',
  },
  taskProject: {
    fontSize: 14,
    color: '#6b7280',
    marginTop: 2,
  },
  taskStatus: {
    backgroundColor: '#f3f4f6',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 12,
  },
  statusText: {
    fontSize: 12,
    color: '#374151',
  },
  actionButtons: {
    flexDirection: 'row',
    justifyContent: 'space-around',
  },
  actionButton: {
    flex: 1,
    marginHorizontal: 4,
  },
});
