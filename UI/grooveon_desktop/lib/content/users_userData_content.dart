import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/users_screen.dart';

class UsersDataContent extends StatelessWidget {
  const UsersDataContent({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: UsersScreen.cardColor,
        border: Border.all(color: UsersScreen.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "User data",
            style: TextStyle(
              fontSize: 30,
              fontWeight: FontWeight.w800,
              color: UsersScreen.textColor,
            ),
          ),
          const SizedBox(height: 18),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            decoration: BoxDecoration(
              color: const Color(0xFFF9F9FB),
              border: Border.all(color: UsersScreen.borderColor),
              borderRadius: BorderRadius.circular(10),
            ),
            child: const Row(
              children: [
                Expanded(
                  flex: 2,
                  child: Text(
                    "Username",
                    style: TextStyle(fontWeight: FontWeight.w700),
                  ),
                ),
                Expanded(
                  flex: 3,
                  child: Text(
                    "Email",
                    style: TextStyle(fontWeight: FontWeight.w700),
                  ),
                ),
                Expanded(
                  flex: 2,
                  child: Text(
                    "Type",
                    style: TextStyle(fontWeight: FontWeight.w700),
                  ),
                ),
                Expanded(
                  flex: 2,
                  child: Text(
                    "Status",
                    style: TextStyle(fontWeight: FontWeight.w700),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 10),
          const _UserRow(
            username: "user1234",
            email: "user1@grooveon.com",
            type: "Basic",
            status: "Active",
          ),
          const _UserRow(
            username: "music2026",
            email: "user2@grooveon.com",
            type: "Premium",
            status: "Active",
          ),
          const _UserRow(
            username: "playlist1",
            email: "user4@grooveon.com",
            type: "Basic",
            status: "Inactive",
          ),
        ],
      ),
    );
  }
}

class _UserRow extends StatelessWidget {
  final String username;
  final String email;
  final String type;
  final String status;

  const _UserRow({
    required this.username,
    required this.email,
    required this.type,
    required this.status,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          Expanded(flex: 2, child: Text(username)),
          Expanded(flex: 3, child: Text(email)),
          Expanded(flex: 2, child: Text(type)),
          Expanded(
            flex: 2,
            child: Text(
              status,
              style: TextStyle(
                color: status == "Active" ? Colors.green : Colors.red,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}