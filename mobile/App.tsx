import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createStackNavigator } from '@react-navigation/stack';
import { Provider as PaperProvider } from 'react-native-paper';
import { StatusBar } from 'expo-status-bar';
import { Ionicons } from '@expo/vector-icons';

// Import screens
import { DashboardScreen } from './src/pages/DashboardScreen';
import { ProjectsScreen } from './src/pages/ProjectsScreen';
import { TasksScreen } from './src/pages/TasksScreen';
import { ResourcesScreen } from './src/pages/ResourcesScreen';
import { BookingsScreen } from './src/pages/BookingsScreen';
import { CalendarScreen } from './src/pages/CalendarScreen';
import { LoginScreen } from './src/pages/LoginScreen';

const Tab = createBottomTabNavigator();
const Stack = createStackNavigator();

function TabNavigator() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: keyof typeof Ionicons.glyphMap;

          if (route.name === 'Dashboard') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'Projects') {
            iconName = focused ? 'folder' : 'folder-outline';
          } else if (route.name === 'Tasks') {
            iconName = focused
              ? 'checkmark-circle'
              : 'checkmark-circle-outline';
          } else if (route.name === 'Resources') {
            iconName = focused ? 'calendar' : 'calendar-outline';
          } else if (route.name === 'Bookings') {
            iconName = focused ? 'time' : 'time-outline';
          } else if (route.name === 'Calendar') {
            iconName = focused ? 'calendar' : 'calendar-outline';
          } else {
            iconName = 'help-outline';
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: '#2563eb',
        tabBarInactiveTintColor: 'gray',
        headerShown: false,
      })}
    >
      <Tab.Screen name='Dashboard' component={DashboardScreen} />
      <Tab.Screen name='Projects' component={ProjectsScreen} />
      <Tab.Screen name='Tasks' component={TasksScreen} />
      <Tab.Screen name='Resources' component={ResourcesScreen} />
      <Tab.Screen name='Bookings' component={BookingsScreen} />
      <Tab.Screen name='Calendar' component={CalendarScreen} />
    </Tab.Navigator>
  );
}

export default function App() {
  return (
    <PaperProvider>
      <NavigationContainer>
        <Stack.Navigator initialRouteName='Login'>
          <Stack.Screen
            name='Login'
            component={LoginScreen}
            options={{ headerShown: false }}
          />
          <Stack.Screen
            name='Main'
            component={TabNavigator}
            options={{ headerShown: false }}
          />
        </Stack.Navigator>
      </NavigationContainer>
      <StatusBar style='auto' />
    </PaperProvider>
  );
}
