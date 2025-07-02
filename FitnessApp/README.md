# Fitness Tracking Application

This is a full-stack fitness tracking application that helps Super Admin, coaches (admins) manage clients and their workout plans, while clients can track their progress in real-time.

## Features Overview

### Core Functionalities

**User Management**
- Secure registration and login (JWT authentication)
- Role-based access (`Super Admin`,`Admin` and `Client`)
- Password encryption and token handling

**Workout Plans**
- Coaches create workout plans
- Assign plans to clients
- Clients view active and completed plans
- Mark plans as completed
- View workout history

**Workout Logs**
- Clients log daily workouts
- Attach progress details (e.g., sets, reps, notes)

**Progress Tracking**
- Clients upload progress images
- Track progress over time

**Notifications**
- Real-time notifications using SignalR (e.g., *"You have been assigned a new workout plan"*)
- Email notifications sent to clients
- Toast popups on the client app when new notifications arrive

**Coach-Client Mapping**
- Coaches can see assigned clients
- Clients see their assigned coach

**User Work Tasks**
- Coaches create tasks or challenges
- Clients can view and complete them

**Security**
- JWT authentication with role-based authorization
- CORS configured to allow Angular frontend
- Custom error handling middleware

**Admin Features**
- Manage users (list, delete, update)
- Manage Coach Mapping to Clients
- Assign workout plans to clients
- Monitor progress logs

**Client Features**
- View assigned workout plans
- Log workouts and progress
- Receive real-time updates and emails about plan enrollment
