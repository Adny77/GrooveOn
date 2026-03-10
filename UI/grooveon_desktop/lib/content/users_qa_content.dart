import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/users_screen.dart';

class UsersQaContent extends StatelessWidget {
  const UsersQaContent({super.key});

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
        children: const [
          Text(
            "Q&A",
            style: TextStyle(
              fontSize: 30,
              fontWeight: FontWeight.w800,
              color: UsersScreen.textColor,
            ),
          ),
          SizedBox(height: 18),
          _QaItem(
            question: "How do I upgrade to Premium?",
            answer: "You can upgrade from the subscription page in the mobile app.",
          ),
          SizedBox(height: 12),
          _QaItem(
            question: "How can I reset my password?",
            answer: "Use the forgot password option on the login screen.",
          ),
          SizedBox(height: 12),
          _QaItem(
            question: "Can I change my username?",
            answer: "Username changes can be managed by the admin panel.",
          ),
        ],
      ),
    );
  }
}

class _QaItem extends StatelessWidget {
  final String question;
  final String answer;

  const _QaItem({
    required this.question,
    required this.answer,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            question,
            style: const TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: UsersScreen.textColor,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            answer,
            style: const TextStyle(
              fontSize: 13,
              color: UsersScreen.subTextColor,
            ),
          ),
        ],
      ),
    );
  }
}